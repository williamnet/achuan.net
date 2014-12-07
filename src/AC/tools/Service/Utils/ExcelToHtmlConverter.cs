using System.Collections.Generic;
using System.IO;
using System.Xml;
using NPOI.HSSF.Converter;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace AC.Tools.Service.Utils
{
    /// <summary>
    /// Excelת����Bootstrap��ʽ���
    /// </summary>
    public class ExcelToHtmlConverter
    {
        /// <summary>
        /// html���߶���
        /// </summary>
        private readonly HtmlUtils htmlUtils;

        #region ���캯��

        public ExcelToHtmlConverter()
        {
            var xmlDocument = new XmlDocument();
            htmlUtils = new HtmlUtils(xmlDocument);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// ����theadԪ�أ�����theadԪ���ڵı�����,Ĭ��sheet�е�һ������Ϊ������
        /// </summary>
        /// <param name="headerRow">����������</param>
        /// <returns>
        /// ����������ʾhtml:
        /// <thead>
        ///     <tr>
        ///         <th>001</th>
        ///         <th>002</th>
        ///         <th>003</th>
        ///     </tr>
        /// </thead>
        /// </returns>
        protected XmlElement ProcessThead(IRow headerRow)
        {
            XmlElement thead = htmlUtils.CreateTableHeader();
            XmlElement tr = htmlUtils.CreateTableRow();
            for (int i = headerRow.FirstCellNum; i < headerRow.LastCellNum; i++)
            {
                XmlElement th = htmlUtils.CreateTableHeaderCell();
                th.AppendChild(htmlUtils.CreateText(headerRow.GetCell(i).StringCellValue));
                tr.AppendChild(th);
            }
            thead.AppendChild(tr);
            return thead;
        }

        /// <summary>
        /// ����tbodyԪ�أ�����tbodyԪ���еı������
        /// </summary>
        /// <param name="sheet">sheet����</param>
        /// <returns>
        /// ����������ʾ��html:
        /// <tbody>
        ///     <tr>
        ///         <td>001</td>
        ///         <td>002</td>
        ///         <td>003</td>
        ///     </tr>
        /// </tbody>
        /// </returns>
        protected XmlElement ProcessTbody(ISheet sheet)
        {
            CellRangeAddress[][] mergedRanges = ExcelToHtmlUtils.BuildMergedRangesMap(sheet as HSSFSheet);

            XmlElement tbody = htmlUtils.CreateTableBody();

            for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                if (row == null)
                {
                    continue;
                }
                if (row.LastCellNum <= 0)
                {
                    continue;
                }

                XmlElement tr = htmlUtils.CreateTableRow();
                for (int j = row.FirstCellNum; j < (int) row.LastCellNum; j++)
                {
                    short color = row.GetCell(j).CellStyle.GetFont(sheet.Workbook).Color;
                    bool isStrikeout = row.GetCell(j).CellStyle.GetFont(sheet.Workbook).IsStrikeout;
                    short boldweight = row.GetCell(j).CellStyle.GetFont(sheet.Workbook).Boldweight;
                    CellRangeAddress range = ExcelToHtmlUtils.GetMergedRange(mergedRanges, row.RowNum, j);
                    if (range != null && (range.FirstColumn != j || range.FirstRow != row.RowNum))
                    {
                        continue;
                    }

                    XmlElement td = htmlUtils.CreateTableCell();
                    if (range != null)
                    {
                        if (range.FirstColumn != range.LastColumn)
                            td.SetAttribute("colspan", (range.LastColumn - range.FirstColumn + 1).ToString());
                        if (range.FirstRow != range.LastRow)
                            td.SetAttribute("rowspan", (range.LastRow - range.FirstRow + 1).ToString());
                    }
                    if (row.GetCell(j) == null || string.IsNullOrEmpty(row.GetCell(j).ToString()))
                    {
                        td.AppendChild(htmlUtils.CreateText(string.Empty));
                    }
                    else
                    {
                        if (color == 10) //��ɫ
                        {
                            td.AppendChild(htmlUtils.CreateCode());
                        }
                        if (isStrikeout) //ɾ����
                        {
                            td.AppendChild(htmlUtils.CreateStrikeout());
                        }
                        if (boldweight == 700) //����
                        {
                            td.AppendChild(htmlUtils.CreateStrong());
                        }
                        td.AppendChild(htmlUtils.CreateText(row.GetCell(j).ToString()));
                    }

                    tr.AppendChild(td);
                }
                tbody.AppendChild(tr);
            }
            return tbody;
        }

        /// <summary>
        /// ��ȡһ��excel�е�sheet��Ӧ��bootstrap��ʽ��html����ı���Ĭ�����ü������ɫ
        /// </summary>
        /// <param name="useSection">�Ƿ��ڱ������ʾ�½�</param>
        /// <param name="sheet">excel��һ��sheet</param>
        /// <returns>����sheet��Ӧ��bootstrap��ʽ��html����ı�</returns>
        protected string GetHtmlOfSheet(bool useSection, ISheet sheet)
        {
            return GetHtmlOfSheet(useSection, sheet, true);
        }

        /// <summary>
        /// ��ȡһ��excel�е�sheet��Ӧ��bootstrap��ʽ��html����ı�
        /// </summary>
        /// <param name="useSection">�Ƿ��ڱ������ʾ�½�</param>
        /// <param name="sheet">excel��һ��sheet</param>
        /// <param name="useStripted">�Ƿ��ڱ�������ü������ɫ��������Ϊǳ��ɫ��</param>
        /// <returns>����sheet��Ӧ��bootstrap��ʽ��html����ı�</returns>
        protected string GetHtmlOfSheet(bool useSection, ISheet sheet, bool useStripted)
        {
            //����һ��divԪ��
            XmlElement root = htmlUtils.CreateBlock();

            /* �������Ϊ����һ��sectionƬ�Σ�����html���£�
             * <div id="sheetname">
             *      <div class="page-header">
             *          <h2>sheetName</h2>
             *      </div>
             * </div>*/
            //����һ��sectionԪ�أ�����sheetNameָ��Ϊ��section��id����ֵ
            XmlElement section = ProcessSection(sheet.SheetName);
            //����һ�δ���
            XmlElement pageHeader = ProcessPageHeader(sheet.SheetName);
            section.AppendChild(pageHeader);

            //����tableԪ��
            XmlElement table = ProcessTable(useStripted);
            //����theadԪ�أ�����������
            XmlElement thead = ProcessThead(sheet.GetRow(0));
            //����tbodyԪ�أ������������
            XmlElement tbody = ProcessTbody(sheet);

            table.AppendChild(thead);
            table.AppendChild(tbody);

            if (useSection) //�����Ҫʹ���½ڣ���ѱ��html׷�ӵ�section��
            {
                section.AppendChild(table);
                root.AppendChild(section);
            }
            else //���򣬱��html׷�ӵ�div��
            {
                root.AppendChild(table);
            }
            return root.InnerXml;
        }

        /// <summary>
        /// ��������sectionԪ��
        /// </summary>
        /// <param name="sectionId">ָ����sectionId</param>
        /// <returns>
        /// ����������ʾhtml����
        /// &lt;section id=""&gt;&lt;/section&gt;
        /// </returns>
        protected XmlElement ProcessSection(string sectionId)
        {
            XmlElement section = htmlUtils.CreateSection();
            section.SetAttribute("id", sectionId);
            return section;
        }

        /// <summary>
        /// ����һ��pageheaderԪ�ض��䣬������ʾһ��section
        /// </summary>
        /// <param name="sectionName"></param>
        /// <returns>
        /// <div class="page-header">
        ///     <h2>sectionName</h2>
        /// </div>
        /// </returns>
        protected XmlElement ProcessPageHeader(string sectionName)
        {
            XmlElement pageHeader = htmlUtils.CreateBlock();
            pageHeader.SetAttribute("class", "page-header");
            XmlElement header2 = htmlUtils.CreateHeader2();
            header2.AppendChild(htmlUtils.CreateText(sectionName));
            pageHeader.AppendChild(header2);
            return pageHeader;
        }

        /// <summary>
        /// ����tableԪ��
        /// </summary>
        /// <param name="useStriped">�Ƿ�����table-striped����</param>
        /// <returns>����<table class=""></table>��ʾ��html</returns>
        protected XmlElement ProcessTable(bool useStriped)
        {
            string tableStriped = useStriped ? " table-striped" : string.Empty;
            XmlElement table = htmlUtils.CreateTable();
            table.SetAttribute("class", string.Format("table table-bordered table-condensed{0}", tableStriped));
            return table;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// ��ȡExcel��һ��sheetת����bootstrap��ʽ���֮���html table���
        /// </summary>
        /// <param name="excelFilePath">excel�ļ�·��</param>
        /// <param name="useSection">�Ƿ��ڱ������ʾ�½�</param>
        /// <returns>������bootstrap��ʽ��ʾ��html�ַ���</returns>
        public string GetFirstSheetHtml(string excelFilePath, bool useSection)
        {
            if (!File.Exists(excelFilePath))
            {
                throw new FileNotFoundException("file not found:" + excelFilePath);
            }

            using (var fileStream = new FileStream(excelFilePath, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = WorkbookFactory.Create(fileStream);

                ISheet sheet = workbook.GetSheetAt(0);

                return GetHtmlOfSheet(useSection, sheet);
            }
        }

        /// <summary>
        /// ��ȡexcel����sheetת����bootstrap��ʽ���֮���html�ı�
        /// </summary>
        /// <param name="excelFilePath">excel�ļ�·��</param>
        /// <param name="useSection">�Ƿ��ڱ������ʾ�½�</param>
        /// <returns>����һ�������ֵ䣬��ʾsheetname��html��ʾ���ֵ���</returns>
        public Dictionary<string, string> GetAllSheetHtml(string excelFilePath, bool useSection)
        {
            return GetAllSheetHtml(excelFilePath, useSection, true);
        }

        /// <summary>
        /// ��ȡexcel����sheetת����bootstrap��ʽ���֮���html�ı�
        /// </summary>
        /// <param name="excelFilePath">excel�ļ�·��</param>
        /// <param name="useSection">�Ƿ��ڱ������ʾ�½�</param>
        /// <param name="useStriped">�Ƿ��ڱ�������ü������ɫ��������Ϊǳ��ɫ��</param>
        /// <returns>����һ�������ֵ䣬��ʾsheetname��html��ʾ���ֵ���</returns>
        public Dictionary<string, string> GetAllSheetHtml(string excelFilePath, bool useSection, bool useStriped)
        {
            if (!File.Exists(excelFilePath))
            {
                throw new FileNotFoundException("file not found:" + excelFilePath);
            }
            var result = new Dictionary<string, string>();
            using (var fileStream = new FileStream(excelFilePath, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = WorkbookFactory.Create(fileStream);
                for (int i = 0; i < workbook.NumberOfSheets; i++)
                {
                    ISheet sheet = workbook.GetSheetAt(i);
                    string htmlOfSheet = GetHtmlOfSheet(true, sheet, useStriped);
                    result.Add(sheet.SheetName, htmlOfSheet);
                }
            }
            return result;
        }

        #endregion
    }
}
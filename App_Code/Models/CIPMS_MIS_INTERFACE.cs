using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
///CIPMS_MIS_INTERFACE 的摘要说明
/// </summary>
/// <summary>
/// CIPMS_MIS_INTERFACE:实体类(属性说明自动提取数据库字段的描述信息)
/// </summary>
namespace Model
{
    [Serializable]
    public partial class CIPMS_MIS_INTERFACE
    {
        public CIPMS_MIS_INTERFACE()
        { }
        #region Model
        private decimal _id;
        private string _factory_code;
        private string _customer;
        private string _garment_order_no;
        private string _job_order_no;
        private decimal? _lay_no;
        private decimal? _bundle_no;
        private string _carton_barcode;
        private string _bundle_barcode;
        private string _part_code;
        private string _color_code;
        private string _size_code;
        private decimal? _output_qty;
        private decimal? _reduce_qty;
        private DateTime? _send_on;
        private string _send_by;
        private DateTime? _receive_on;
        private string _receive_by;
        private string _type;
        private string _transfer_desc;
        private string _status;
        private string _docno;
        private string _printpart;
        private string _carno;
        /// <summary>
        /// 
        /// </summary>
        public decimal ID
        {
            set { _id = value; }
            get { return _id; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string FACTORY_CODE
        {
            set { _factory_code = value; }
            get { return _factory_code; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string CUSTOMER
        {
            set { _customer = value; }
            get { return _customer; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string GARMENT_ORDER_NO
        {
            set { _garment_order_no = value; }
            get { return _garment_order_no; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string JOB_ORDER_NO
        {
            set { _job_order_no = value; }
            get { return _job_order_no; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? LAY_NO
        {
            set { _lay_no = value; }
            get { return _lay_no; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? BUNDLE_NO
        {
            set { _bundle_no = value; }
            get { return _bundle_no; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string CARTON_BARCODE
        {
            set { _carton_barcode = value; }
            get { return _carton_barcode; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string BUNDLE_BARCODE
        {
            set { _bundle_barcode = value; }
            get { return _bundle_barcode; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string PART_CODE
        {
            set { _part_code = value; }
            get { return _part_code; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string COLOR_CODE
        {
            set { _color_code = value; }
            get { return _color_code; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string SIZE_CODE
        {
            set { _size_code = value; }
            get { return _size_code; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? OUTPUT_QTY
        {
            set { _output_qty = value; }
            get { return _output_qty; }
        }
        /// <summary>
        /// 
        /// </summary>
        public decimal? REDUCE_QTY
        {
            set { _reduce_qty = value; }
            get { return _reduce_qty; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? SEND_ON
        {
            set { _send_on = value; }
            get { return _send_on; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string SEND_BY
        {
            set { _send_by = value; }
            get { return _send_by; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? RECEIVE_ON
        {
            set { _receive_on = value; }
            get { return _receive_on; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string RECEIVE_BY
        {
            set { _receive_by = value; }
            get { return _receive_by; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string TYPE
        {
            set { _type = value; }
            get { return _type; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string TRANSFER_DESC
        {
            set { _transfer_desc = value; }
            get { return _transfer_desc; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string STATUS
        {
            set { _status = value; }
            get { return _status; }
        }
        public string DOC_NO
        {
            set { _docno = value; }
            get { return _docno; }
        }
        public string PRINT_PART
        {
            set { _printpart = value; }
            get { return _printpart; }
        }
        public string CAR_NO
        {
            set { _carno = value; }
            get { return _carno; }
        }
        #endregion Model

    }

    public class CIPMS_PRT_TO_CIPMS
    {
        public string BARCODE { get; set; }
        public string TO_PROCESS { get; set; }
        public string PART_LIST { get; set; }
        public string REMAIN_PART_LIST { get; set; }
        public string DISCREPANCY_QTY { get; set; }
        public string BATCH_NO { get; set; }
        public string FACTORY_CD { get; set; }
        public string GO_NO { get; set; }
        public string JO_NO { get; set; }
        public string CREATE_DATE { get; set; }
        public string STATUS { get; set; }
        public string REMARK { get; set; }
        public string DOC_NO { get; set; }
        public string PRINT_PART { get; set; }
        public string CAR_NO { get; set; }
        
    }
}
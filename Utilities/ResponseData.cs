namespace DVG.WIS.Utilities
{
    public class ResponseData
    {
        private string _content = string.Empty;
        private object _data;
        private int _errorCode;
        private string _message = string.Empty;
        private bool _success;
        private int _totalRow;

        public static ResponseData CreateResponseData()
        {
            return new ResponseData
            {
                Success = false,
                Message = "",
                Data = null,
                TotalRow = 0,
                ErrorCode = 0,
                Content = ""
            };
        }

        public string Content
        {
            get
            {
                return this._content;
            }
            set
            {
                this._content = value;
            }
        }

        public object Data
        {
            get
            {
                return this._data;
            }
            set
            {
                this._data = value;
            }
        }

        public int ErrorCode
        {
            get
            {
                return this._errorCode;
            }
            set
            {
                this._errorCode = value;
            }
        }

        public string Message
        {
            get
            {
                return this._message;
            }
            set
            {
                this._message = value;
            }
        }

        public bool Success
        {
            get
            {
                return this._success;
            }
            set
            {
                this._success = value;
            }
        }

        public int Total { get; set; }

        public int TotalRow
        {
            get
            {
                return this._totalRow;
            }
            set
            {
                this._totalRow = value;
            }
        }

        public int Type { get; set; }
        public string Token { get; set; }
        public bool RefreshToken { get; set; }
    }
}

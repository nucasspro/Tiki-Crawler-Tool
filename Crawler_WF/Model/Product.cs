namespace Crawler_WF
{
    public class Product
    {
        private string _id;
        private string _name;
        private string _sku;
        private string _salePrice;
        private string _regularPrice;
        private string _category;
        private string _brand;
        private string _shortInfo;
        private string _longInfo;
        private string _description;
        private string _image;
        private string _link;

        public string Id { get { return _id; } set { _id = value; } }
        public string Name { get { return _name; } set { _name = value; } }
        public string Sku { get { return _sku; } set { _sku = value; } }
        public string SalePrice { get { return _salePrice; } set { _salePrice = value; } }
        public string RegularPrice { get { return _regularPrice; } set { _regularPrice = value; } }
        public string Category { get { return _category; } set { _category = value; } }
        public string Brand { get { return _brand; } set { _brand = value; } }
        public string ShortInfo { get { return _shortInfo; } set { _shortInfo = value; } }
        public string LongInfo { get { return _longInfo; } set { _longInfo = value; } }
        public string Description { get { return _description; } set { _description = value; } }
        public string Image { get { return _image; } set { _image = value; } }
        public string Link { get { return _link; } set { _link = value; } }


        public Product()
        {
        }

        public Product(string id, string name, string sku, string salePrice, string regularPrice, string category, string brand, string shortInfo, string longInfo, string description, string image, string link)
        {
            this._id = id;
            this._name = name;
            this._sku = sku;
            this._salePrice = salePrice;
            this._regularPrice = regularPrice;
            this._category = category;
            this._brand = brand;
            this._shortInfo = shortInfo;
            this._longInfo = longInfo;
            this._description = description;
            this._image = image;
            this._link = link;
        }
    }
}
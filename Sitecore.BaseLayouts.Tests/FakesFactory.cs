using NSubstitute;
using Sitecore.BaseLayouts.Data;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.FakeDb;

namespace Sitecore.BaseLayouts.Tests
{
    public class FakesFactory
    {
        private readonly Db _db;

        public FakesFactory(Db db)
        {
            _db = db;
        }

        public Field CreateFakeEmptyField()
        {
            _db.Add(new DbItem("Home") {{"Title", null}});
            var item = _db.GetItem("/sitecore/content/home");
            return item.Fields["Title"];
        }

        public Field CreateFakeLayoutField(ID id = null, ID parentId = null, string layoutValue = null,
            string finalLayoutValue = null,
            ID baseLayoutId = null,
            bool addBaseLayoutField = true)
        {
            return
                CreateFakeItem(id, parentId, layoutValue, finalLayoutValue, baseLayoutId, addBaseLayoutField).Fields[
                    FieldIDs.LayoutField];
        }

#if FINAL_LAYOUT
        public Field CreateFakeFinalLayoutField(ID id = null, ID parentId = null, string layoutValue = null,
            string finalLayoutValue = null,
            ID baseLayoutId = null,
            bool addBaseLayoutField = true)
        {
            return
                CreateFakeItem(id, parentId, layoutValue, finalLayoutValue, baseLayoutId, addBaseLayoutField).Fields[
                    FieldIDs.FinalLayoutField];
        }
#endif

        public Item CreateFakeItem(ID id = null, ID parentId = null, string layoutValue = null,
            string finalLayoutValue = null,
            ID baseLayoutId = null,
            bool addBaseLayoutField = true)
        {
            if (ID.IsNullOrEmpty(id))
            {
                id = new ID();
            }

            if (ID.IsNullOrEmpty(parentId))
            {
                parentId = ItemIDs.ContentRoot;
            }

            if (layoutValue == null)
            {
                layoutValue =
                    "<r xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><d id=\"{FE5D7FDF-89C0-4D99-9AA3-B5FBD009C9F3}\" l=\"{14030E9F-CE92-49C6-AD87-7D49B50E42EA}\"><r id=\"{885B8314-7D8C-4CBB-8000-01421EA8F406}\" ph=\"main\" uid=\"{43222D12-08C9-453B-AE96-D406EBB95126}\" /><r id=\"{CE4ADCFB-7990-4980-83FB-A00C1E3673DB}\" ph=\"/main/centercolumn\" uid=\"{CF044AD9-0332-407A-ABDE-587214A2C808}\" /></d></r>";
            }

            if (finalLayoutValue == null)
            {
                finalLayoutValue =
                    "<r xmlns:p=\"p\" xmlns:s=\"s\" p:p=\"1\"><d id=\"{FE5D7FDF-89C0-4D99-9AA3-B5FBD009C9F3}\"><r uid=\"{BC07E3F3-046F-4742-AF4A-E0D22C9B600A}\" s:id=\"{493B3A83-0FA7-4484-8FC9-4680991CF743}\" s:ph=\"/main/centercolumn/content\" /></d></r>";
            }

            var dbItem = new DbItem(id.ToShortID().ToString(), id)
            {
                ParentID = parentId,
                Fields =
                {
                    {FieldIDs.LayoutField, layoutValue}
#if FINAL_LAYOUT
                    ,
                    {FieldIDs.FinalLayoutField, finalLayoutValue}
#endif
                }
            };

            if (addBaseLayoutField)
            {
                dbItem.Fields.Add(BaseLayoutItem.BaseLayoutFieldId,
                    ID.IsNullOrEmpty(baseLayoutId) ? null : baseLayoutId.ToString());
            }

            _db.Add(dbItem);

            var item = _db.GetItem(id);
            return item;
        }
    }
}
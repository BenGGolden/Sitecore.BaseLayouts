using Sitecore.Data;

namespace Sitecore.BaseLayouts.Tests
{
    using Sitecore.Data.Fields;
    using Sitecore.FakeDb;

    public class FakeFieldFactory
    {
        public Field CreateFakeEmptyField(Db db, string databaseName = null)
        {
            if (string.IsNullOrWhiteSpace(databaseName))
            {
                databaseName = "master";
            }

            db.Add(new DbItem("Home") { { "Title", null } });
            var item = db.GetItem("/sitecore/content/home");
            return item.Fields["Title"];
        }

        public Field CreateFakeLayoutField(Db db, ID id = null, ID parentId = null, string layoutValue = null,
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

            if (string.IsNullOrEmpty(layoutValue))
            {
                layoutValue =
                    "<?xml version=\"1.0\" encoding=\"utf-16\"?><r xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><d id=\"{FE5D7FDF-89C0-4D99-9AA3-B5FBD009C9F3}\" l=\"{14030E9F-CE92-49C6-AD87-7D49B50E42EA}\"><r id=\"{885B8314-7D8C-4CBB-8000-01421EA8F406}\" ph=\"main\" uid=\"{43222D12-08C9-453B-AE96-D406EBB95126}\" /><r id=\"{CE4ADCFB-7990-4980-83FB-A00C1E3673DB}\" ph=\"/main/centercolumn\" uid=\"{CF044AD9-0332-407A-ABDE-587214A2C808}\" /><r id=\"{493B3A83-0FA7-4484-8FC9-4680991CF743}\" ph=\"content\" uid=\"{B343725A-3A93-446E-A9C8-3A2CBD3DB489}\" /></d></r>";
            }

            var dbItem = new DbItem(id.ToShortID().ToString(), id)
            {
                ParentID = parentId,
                Fields = {{FieldIDs.LayoutField, layoutValue}}
            };

            if (addBaseLayoutField)
            {
                dbItem.Fields.Add(BaseLayoutSettings.FieldId,
                    ID.IsNullOrEmpty(baseLayoutId) ? null : baseLayoutId.ToString());
            }

            db.Add(dbItem);

            var item = db.GetItem(id);
            return item.Fields[FieldIDs.LayoutField];
        }
    }
}
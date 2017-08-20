using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Events.Hooks;
using Sitecore.SecurityModel;

namespace Sitecore.Support
{
    public class CreateCommerceCatalogEnumerationControlItem : IHook
    {
        public void Initialize()
        {
            using (new SecurityDisabler())
            {
                var databaseName = "core";
                var itemPath = "/sitecore/client/Applications/MerchandisingManager/Settings/System/ControlMapping/Commerce Catalog Enumeration Control";
                var itemName = "Commerce Catalog Enumeration Control";
                
                var database = Factory.GetDatabase(databaseName);
                var item = database.GetItem(itemPath);
                
                if (item != null)
                {
                    // already installed
                    return;
                }

                var parentItemPath = "/sitecore/client/Applications/MerchandisingManager/Settings/System/ControlMapping";
                var parentItem = database.GetItem(parentItemPath);
                var templatePath = "/sitecore/client/Applications/MerchandisingManager/Settings/Templates/ControlMapping";
                var templateItem = database.GetTemplate(templatePath);

                Assert.IsNotNull(parentItem, typeof(Item));
                Assert.IsNotNull(templateItem, typeof(TemplateItem));

                Log.Info($"Creating {itemName}", this);
                item = parentItem.Add(itemName, templateItem);
                item.Editing.BeginEdit();
                item["View Control"] = "/sitecore/client/Business Component Library/version 1/Layouts/Renderings/Common/Text"; // {7717EB6C-9F90-4C58-826D-5E87722A0318}
                item["View Binding Property"] = "Text";
                item["Column Width"] = "3";
                item["Edit Control"] = "/sitecore/client/Business Component Library/version 1/Layouts/Renderings/Common/ComboBox"; // {25D357D3-3648-4336-9B53-55EED7CFA078}
                item["Edit Binding Property"] = "selectedValue";
                item.Editing.EndEdit();
            }
        }
    }
}
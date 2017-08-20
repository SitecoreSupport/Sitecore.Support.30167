using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using Sitecore.Commerce.Connect.CommerceServer.Controls;
using Sitecore.Web;

namespace Sitecore.Support.Commerce.UX.Merchandising
{
    public class CommerceSearchSupportController : Controller
    {
        private class CommerceSearchSupportResultItem
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        private string GetRequestedCommerceId()
        {
            return WebUtil.GetFormValue("id");
        }

        private JsonResult GetEmptyJsonResponse()
        {
            Dictionary<string, List<CommerceSearchSupportResultItem>> emptyResult = new Dictionary<string, List<CommerceSearchSupportResultItem>>();

            return base.Json(emptyResult);
        }

        private object InvokeRestrictedInstanceMethods(object instance, string methodName, object[] parameters)
        {
            BindingFlags eFlags = BindingFlags.Instance | BindingFlags.NonPublic;
            MethodInfo mInfoMethod = instance.GetType().GetMethod(methodName, eFlags);
            return mInfoMethod.Invoke(instance, parameters);
        }

        private List<CommerceSearchSupportResultItem> GetResultItemsFromFieldValues(List<string> fieldValues)
        {
            return fieldValues.Select(f => new CommerceSearchSupportResultItem()
            {
                Name = f,
                Value = f
            }).ToList();
        }

        private List<string> GetValues(string templateId, string fieldName)
        {
            CommerceCatalogEnumerationControl commerceCatalogControl = new CommerceCatalogEnumerationControl();
            var template = Sitecore.Context.ContentDatabase.GetTemplate(templateId);
            commerceCatalogControl.FieldId = template.GetField(fieldName).ID.ToString();

            return ((IEnumerable<string>)InvokeRestrictedInstanceMethods(commerceCatalogControl, "GetItems", null)).ToList();
        }

        private Dictionary<string, List<CommerceSearchSupportResultItem>> GetValuesByItemId(string itemId, string[] multipleFieldNames)
        {
            Dictionary<string, List<CommerceSearchSupportResultItem>> result = new Dictionary<string, List<CommerceSearchSupportResultItem>>();

            foreach (string fieldName in multipleFieldNames)
            {
                var templateId = Sitecore.Context.ContentDatabase.GetItem(itemId).TemplateID;
                result.Add(fieldName, GetResultItemsFromFieldValues(GetValues(templateId.ToString(), fieldName)));
            }

            return result;
        }

        private Dictionary<string, List<CommerceSearchSupportResultItem>> GetValuesByTemplateId(string templateId, string[] multipleFieldNames)
        {
            Dictionary<string, List<CommerceSearchSupportResultItem>> result = new Dictionary<string, List<CommerceSearchSupportResultItem>>();

            foreach (string fieldName in multipleFieldNames)
            {
                result.Add(fieldName, GetResultItemsFromFieldValues(GetValues(templateId, fieldName)));
            }

            return result;
        }

        public JsonResult GetMultipleChoiceFieldValuesByItemId([FromBody]string[] multipleFieldNames)
        {
            string itemId = this.GetRequestedCommerceId();
            if (string.IsNullOrWhiteSpace(itemId))
            {
                return this.GetEmptyJsonResponse();
            }

            var result = this.GetValuesByItemId(itemId, multipleFieldNames);
            return base.Json(result);
        }

        public JsonResult GetMultipleChoiceFieldValuesByTemplateId([FromBody]string[] multipleFieldNames)
        {
            string templateId = this.GetRequestedCommerceId();
            if (string.IsNullOrWhiteSpace(templateId))
            {
                return this.GetEmptyJsonResponse();
            }

            var result = this.GetValuesByTemplateId(templateId, multipleFieldNames);
            return base.Json(result);
        }
    }
}

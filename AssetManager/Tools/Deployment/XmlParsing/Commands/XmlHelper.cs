using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AssetManager.Tools.Deployment.XmlParsing.Commands
{
    public static class XmlHelper
    {
        /// <summary>
        /// Returns the elements internal value or the elements "Value" attribute depending on which one is present.
        /// </summary>
        /// <param name="promptElement"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">
        /// If neither or both internal value and "Value" attributes are present. 
        /// </exception>
        public static string GetElementValueOrValueAttrib(XElement promptElement)
        {
            var elementValue = promptElement.Value;
            var attributeValue = GetAttribute(promptElement, "Value");

            if (!string.IsNullOrEmpty(elementValue) && !string.IsNullOrEmpty(attributeValue))
                throw new InvalidOperationException("Both internal value and 'Value' attributes are present. Make sure the element is populated with only one.");

            if (string.IsNullOrEmpty(elementValue) && string.IsNullOrEmpty(attributeValue))
                throw new InvalidOperationException("Neither internal value or 'Value' attributes are present. Make sure the element is populated with atleast one.");


            if (!string.IsNullOrEmpty(elementValue))
            {
                return elementValue.Trim();
            }
            else
            {
                return attributeValue.Trim();
            }
        }

        /// <summary>
        /// Gets the attribute value with the specifed name from the specified element. Not case sensitive.
        /// </summary>
        /// <param name="element">Target element.</param>
        /// <param name="name">The name of the attribute to return the value from.</param>
        /// <returns></returns>
        public static string GetAttribute(XElement element, string name)
        {
            try
            {
                return element.Attributes().Where(a => a.Name.LocalName.ToUpper() == name.ToUpper()).First().Value;
            }
            catch
            {
                return null;
            }
        }

        public static string GetCommandText(XElement cmd)
        {
            var stringElement = cmd.Element("String");

            if (stringElement != null)
            {
                return stringElement.Value.Trim();
            }

            return string.Empty;
        }
    }
}

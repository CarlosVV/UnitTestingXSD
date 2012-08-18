using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using Xunit;

namespace UnitTests
{
    public class XmlSchemaTests
    {
        private static readonly XmlSchema schemaUnderTest;
        private static readonly XmlSchema helperSchema;

        static XmlSchemaTests()
        {
            string helperSchemaString =                 
                @"<?xml version=""1.0"" encoding=""utf-8""?>" +
                @"<xs:schema attributeFormDefault=""unqualified"" elementFormDefault=""qualified"" xmlns:xs=""http://www.w3.org/2001/XMLSchema"">" +
                @"  <xs:element name=""Setting"" type=""Setting_Type"" />" +
                @"  <xs:element name=""ServiceTypeInitializer"" type=""ServiceTypeInitializer_Type"" />" +
                @"</xs:schema>";

            using (var reader = XmlReader.Create("Settings.xsd"))
            {
                schemaUnderTest = XmlSchema.Read(reader, null);
            }

            helperSchema = XmlSchema.Read(XmlReader.Create(new StringReader(helperSchemaString)), null);
        }

        #region Tests for Setting_Type type

        [Fact]
        public void Schema_accepts_Setting_Type_with_Name_and_value_attributes()
        {
            Assert.True(
                !RunValidation(@"<Setting Name=""Setting1"" Value=""ABC"" />", true).Any());
        }

        [Fact]
        public void Schema_rejects_Setting_Type_without_Name()
        {
            var error = 
                RunValidation(@"<Setting Value=""ABC"" />", true)
                .Single();

            Assert.Equal(XmlSeverityType.Error, error.Severity);
            Assert.Equal(
                "The required attribute 'Name' is missing.",
                error.Message);
        }

        // Other test cases:
        //   Schema_rejects_Setting_Type_without_Value
        //   Schema_rejects_Setting_Type_without_any_attributes
        //   Schema_rejects_Setting_Type_with_undeclared_attributes
        //   Schema_rejects_Setting_Type_with_child_elements
        //   Schema_rejects_Setting_Type_with_non_empty_content

        #endregion

        #region Tests for ServiceTypeInitializer_Type

        [Fact]
        public void Schema_accepts_ServiceTypeInitializer_Type_with_Type_attribute_and_child_Setting_elements()
        {
            Assert.True(
                !RunValidation(
                    @"<ServiceTypeInitializer Type=""ServiceProvider"">" + 
                    @"    <Setting Name=""Setting1"" Value=""ABC"" />" +
                    @"    <Setting Name=""Setting2"" Value=""XYZ"" />" + 
                    @"</ServiceTypeInitializer>", 
                    true).Any());
        }

        [Fact]
        public void Schema_rejects_ServiceTypeInitializer_Type_without_Type_attribute()
        {
            var error = 
                RunValidation(
                    @"<ServiceTypeInitializer>" + 
                    @"    <Setting Name=""Setting1"" Value=""ABC"" />" + 
                    @"</ServiceTypeInitializer>", 
                    true).Single();

            Assert.Equal(XmlSeverityType.Error, error.Severity);
            Assert.Equal(
                "The required attribute 'Type' is missing.",
                error.Message);
        }

        // Other test cases:
        //   Schema_rejects_ServiceTypeInitializer_Type_with_undeclared_attributes_Type_attribute
        //   Schema_rejects_ServiceTypeInitializer_Type_without_Setting_child_elements
        //   Schema_rejects_ServiceTypeInitializer_Type_with_undeclared_child_elements
        //   Schema_rejects_ServiceTypeInitializer_Type_with_content

        #endregion

        #region Tests for Settings type

        [Fact]
        public void Schema_rejects_non_Settings_root_element()
        {
            var elementNames = new[] 
                { 
                    "random", 
                    "ServiceProvider", 
                    "Setting", 
                    "Factory" 
                };

            foreach (var elementName in elementNames)
            {
                var error = 
                    RunValidation(
                        string.Format("<{0} />", elementName), 
                        false)
                    .Single();

                Assert.Equal(XmlSeverityType.Error, error.Severity);
                Assert.Equal(
                    string.Format("The '{0}' element is not declared.", elementName),
                    error.Message);
            }
        }

        [Fact]
        public void Schema_rejects_Settings_root_element_in_non_empty_namespace()
        {
            var error =
                RunValidation("<Settings xmlns='foo' />", false)
                .Single();

            Assert.Equal(XmlSeverityType.Warning, error.Severity);
            Assert.Equal(
                "Could not find schema information for the element 'foo:Settings'.", 
                error.Message);
        }

        [Fact]
        public void Schema_accepts_minimal_valid_Xml()
        {
            Assert.True(!RunValidation("<Settings />", false).Any());
        }

        // Other test cases:
        //   Schema_accepts_Settings_element_with_ServiceProvider_child_element
        //   Schema_accepts_Settings_element_with_Factory_child_element
        //   Schema_accepts_Settings_element_with_ServiceProvider_and_Factory_child_elements
        //   Schema_rejects_Settings_element_with_Factory_and_ServiceProvider_child_elements (invalid order)
        //   Schema_rejects_Settings_element_with_multiple_ServiceProvider_child_elements
        //   Schema_rejects_Settings_element_with_multiple_Factory_child_elements
        //   Schema_rejects_Settings_element_with_undefined_child_element
        //   Schema_rejects_Settings_element_with_content

        #endregion

        private static IEnumerable<ValidationEventArgs> RunValidation(string inputXml, bool includeHelperSchema)
        {
            var schemaSet = new XmlSchemaSet();
            schemaSet.Add(schemaUnderTest);

            if (includeHelperSchema)
            {
                schemaSet.Add(helperSchema);
            }

            var readerSettings = new XmlReaderSettings()
            {
                Schemas = schemaSet,
                ValidationType = ValidationType.Schema,
                ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings,
            };

            var events = new List<ValidationEventArgs>();
            readerSettings.ValidationEventHandler += (s, e) => { events.Add(e); };

            using (var reader = XmlReader.Create(new StringReader(inputXml), readerSettings))
            {
                while (reader.Read())
                    ;
            }

            return events;
        }
    }
}

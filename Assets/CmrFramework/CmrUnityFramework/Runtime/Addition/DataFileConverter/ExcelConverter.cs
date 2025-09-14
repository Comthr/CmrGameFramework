using System.Collections.Generic;

namespace CmrUnityGameFramework.Runtime
{
    public class ExcelConverter : DataFileConverterBase, IDataFileConverter
    {
        private const string SOURCEEXTENSION = "xlsx";
        private DataFileConverterOutputType[] SUPPORTEDOUTPUTTYPES = new DataFileConverterOutputType[]
        {
            DataFileConverterOutputType.Json,
            DataFileConverterOutputType.Xml
        };
        public override string SourceExtension => SOURCEEXTENSION;

        public override IEnumerable<DataFileConverterOutputType> SupportedOutputTypes => SUPPORTEDOUTPUTTYPES;

        public string Convert(DataFileConverterOutputType outputType,string filePath, out string output)
        {
            switch (outputType)
            {
                case DataFileConverterOutputType.Json:

                    break;
                case DataFileConverterOutputType.Xml:

                    break;
            }
            output = null;
            return null;
        }
    }
}

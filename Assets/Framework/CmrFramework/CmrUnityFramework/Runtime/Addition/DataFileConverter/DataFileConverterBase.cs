using System.Collections.Generic;

namespace CmrUnityFramework.Runtime
{
    public abstract class DataFileConverterBase
    {
        /// <summary>
        /// 当前转换器能处理的原始文件扩展名（例如 ".csv", ".txt", ".xlsx"）
        /// </summary>
        public abstract string SourceExtension { get; }

        /// <summary>
        /// 当前转换器支持的输出格式（例如 "json", "xml"）
        /// </summary>
        public abstract IEnumerable<DataFileConverterOutputType> SupportedOutputTypes { get; }
    }
}

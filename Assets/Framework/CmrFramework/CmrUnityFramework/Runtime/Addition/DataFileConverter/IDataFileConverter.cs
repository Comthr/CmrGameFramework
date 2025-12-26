using System.Collections.Generic;

namespace CmrUnityFramework.Runtime
{
    /// <summary>
    /// 数据文件转化类接口
    /// </summary>
    public interface IDataFileConverter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="outputType"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public string Convert(DataFileConverterOutputType outputType,string filePath, out string output);
    }
}


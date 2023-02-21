using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace API.Filters
{
   [Serializable]
    public class ModelValidationException : Exception
    {
        /// <summary>
        /// ModelValidationException class Constructor
        /// </summary>
        protected ModelValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// ModelValidationException class Constructor
        /// </summary>
        public ModelValidationException() : base()
        {
        }

        /// <summary>
        /// ModelValidationException class Constructor
        /// </summary>
        public ModelValidationException(string message) : base(message)
        {
        }

        /// <summary>
        /// ModelValidationException class Constructor
        /// </summary>
        public ModelValidationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
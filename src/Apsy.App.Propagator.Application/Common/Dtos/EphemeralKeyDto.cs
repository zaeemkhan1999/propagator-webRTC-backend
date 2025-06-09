using System;

namespace Apsy.App.Propagator.Application.Common
{
    public class EphemeralKeyDto
    {
        /// <summary>
        /// Unique identifier for the object.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// String representing the object's type. Objects of the same type share the same value.
        /// </summary>
        public string Object { get; set; }

        /// <summary>
        /// Time at which the object was created. Measured in seconds since the Unix epoch.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Whether this object is deleted or not.
        /// </summary>
        public bool? Deleted { get; set; }

        /// <summary>
        /// Time at which the key will expire. Measured in seconds since the Unix epoch.
        /// </summary>
        public DateTime Expires { get; set; }

        /// <summary>
        /// Has the value <c>true</c> if the object exists in live mode or the value <c>false</c> if
        /// the object exists in test mode.
        /// </summary>
        public bool Livemode { get; set; }

        /// <summary>
        /// The key's secret. You can use this value to make authorized requests to the Stripe API.
        /// </summary>
        public string Secret { get; set; }

        public string RawJson { get; set; }
    }
}

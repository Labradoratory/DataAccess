using System;

namespace Labradoratory.Fetch.Controllers
{
    /// <summary>
    /// Defines the content to include with a 201 Created response.
    /// </summary>
    [Flags]
    public enum CreatedResponseOptions
    {
        /// <summary>Enpty, no content.</summary>
        Empty = 0,

        /// <summary>Include the instance that was created.</summary>
        Instance = 1,

        /// <summary>Include the location header.</summary>
        Location = 2
    }
}

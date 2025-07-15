using System;

namespace Unity.GraphToolkit.Editor
{
    /// <summary>
    /// Base class for clipboard service providers, which are used for copy, paste and duplicate operations.
    /// Duplicate operation does not need the clipboard storage area, but benefits from the serialization
    /// capabilities of the service.
    /// </summary>
    [UnityRestricted]
    internal abstract class ClipboardProvider
    {
        /// <summary>
        /// The MIME type of the data that will be placed on the clipboard. Implementers should use this string
        /// to mark their data as GTF data.
        /// </summary>
        protected const string k_SerializedDataMimeType = "application/vnd.unity.graphtoolkit";

        /// <summary>
        /// The clipboard data.
        /// </summary>
        public abstract string Clipboard { get; set; }

        /// <summary>
        /// Serializes the <paramref name="copyPasteData"/>.
        /// </summary>
        /// <param name="copyPasteData">The data to serialize.</param>
        public abstract void SerializeDataToClipboard(CopyPasteData copyPasteData);

        /// <summary>
        /// Deserializes the data stored in the clipboard.
        /// </summary>
        /// <returns>An instance of <see cref="CopyPasteData"/> created from the clipboard data.</returns>
        public abstract CopyPasteData DeserializeDataFromClipboard();

        /// <summary>
        /// Returns true if the clipboard data can be deserialized.
        /// </summary>
        /// <returns>True if the data can be deserialized.</returns>
        public abstract bool CanDeserializeDataFromClipboard();

        /// <summary>
        /// Duplicates a <see cref="CopyPasteData"/> instance without modifying the clipboard.
        /// </summary>
        /// <param name="copyPasteData">The object to duplicate.</param>
        /// <returns>A copy of <paramref name="copyPasteData"/>.</returns>
        public abstract CopyPasteData Duplicate(CopyPasteData copyPasteData);
    }
}

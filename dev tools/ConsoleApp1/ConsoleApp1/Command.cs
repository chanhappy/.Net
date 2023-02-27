using Newtonsoft.Json;

namespace ConsoleApp1
{
    internal class Command
    {
        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("params")]
        public CommandParams Params { get; set; }
    }

    class CommandParams
    {
        /// <summary>
        /// If true 'reportHeapSnapshotProgress' events will be generated while snapshot is being taken.
        /// </summary>
        [JsonProperty("reportProgress")]
        public bool ReportProgress { get; set; }

        /// <summary>
        /// If true, a raw snapshot without artifical roots will be generated
        /// </summary>
        [JsonProperty("treatGlobalObjectsAsRoots")]
        public bool TreatGlobalObjectsAsRoots { get; set; }
    }
}
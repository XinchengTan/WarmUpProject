using System;
using System.Collections.Generic;

namespace Producer
{

    public struct ErrorRateConfig
    {
        public double? badValue;
        public double? missingField;
        public double? additionalField;

    }

    // Same as DimentionAttribute
    public struct FieldAttributes
    {
        public readonly string name;
        public readonly string typeID;
        public readonly FieldParam param;

        public FieldAttributes(string name, string typeID, FieldParam param)
        {
            this.name = name;
            this.typeID = typeID;
            this.param = param;
        }
    }

    public struct FieldParam
    {
        public double? mean;
        public double? standard_deviation;
        public int? max_len;

        // TODO: add all possible args

    }


    public struct FullConfig
    {
        public int ThreadsCount { get; }
        public int RecordsCount { get; }
        public ErrorRateConfig ErrorRate { get; }
        public List<FieldAttributes> Fields;

        public FullConfig(int threads, int records, ErrorRateConfig errorRate, List<FieldAttributes> fields)
        {
            this.ThreadsCount = threads;
            this.RecordsCount = records;
            this.ErrorRate = errorRate;
            this.Fields = fields;
        }
    }

}

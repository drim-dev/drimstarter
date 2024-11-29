using System.Globalization;

namespace Drimstarter.Common.Grpc.Shared.Utils;

public static class GrpcDecimalUtils
{
    public static string ToGrpcDecimal(this decimal value) => value.ToString(CultureInfo.InvariantCulture);

    public static decimal FromGrpcDecimal(this string value) => decimal.Parse(value, CultureInfo.InvariantCulture);
}

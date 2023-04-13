using System.Runtime.Serialization;

namespace NumericalMethods.Infrastructure.NonLinearEquationsSystems.Exceptions
{
	public class InvariantException : Exception
	{
		public InvariantException()
		{
		}

		public InvariantException(string? message) : base(message)
		{
		}

		public InvariantException(string? message, Exception? innerException) : base(message, innerException)
		{
		}

		protected InvariantException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public static void ThrowIf(bool isViolated, string message)
		{
			if(isViolated) throw new InvalidOperationException(message);
		}
	}
}

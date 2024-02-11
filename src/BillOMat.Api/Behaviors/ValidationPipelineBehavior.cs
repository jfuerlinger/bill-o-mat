using FluentValidation;
using FluentValidation.Results;
using MediatR;
using OneOf;
using OneOf.Types;
using System.Linq.Expressions;
using System.Reflection;

namespace BillOMat.Api.Behaviors;

//public sealed class ValidationPipelineBehavior<TRequest, TResponse>
//        (IEnumerable<IValidator<TRequest>> validators) 
//        : IPipelineBehavior<TRequest, TResponse>
//            where TRequest : notnull
//            where TResponse : IOneOf
//{
//    private static bool s_implicitConversionChecked;
//    private static Func<ValidationFailure, TResponse>? s_implicitConversionFunc;

//    private readonly IEnumerable<IValidator<TRequest>> _validators = validators;

//    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
//    {
//        if (s_implicitConversionFunc is null && !s_implicitConversionChecked)
//        {
//            Type responseType = typeof(TResponse);
//            if (responseType.IsGenericType &&
//                responseType.GenericTypeArguments.Any(t => t == typeof(ValidationFailure)))
//            {
//                MethodInfo? implicitConversionMethod = responseType.GetMethod("op_Implicit", [typeof(ValidationFailure)]);

//                if (implicitConversionMethod is not null)
//                {
//                    ParameterExpression errorsParam = Expression.Parameter(typeof(ValidationFailure), "e");
//                    s_implicitConversionFunc =
//                        Expression.Lambda<Func<ValidationFailure, TResponse>>(Expression.Call(implicitConversionMethod, errorsParam), errorsParam).Compile();
//                }
//            }

//            s_implicitConversionChecked = true;
//        }

//        if (s_implicitConversionFunc is not null)
//        {
//            var context = new ValidationContext<TRequest>(request);

//            ValidationResult[] validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
//            ValidationResult validationResult = new ValidationResult(validationResults);

//            if (!validationResult.IsValid)
//            {
//                IEnumerable<ValidationFailure> errors = 
//                    validationResult.Errors
//                        .Select(e => 
//                                    new ValidationFailure(e.PropertyName, e.ErrorMessage, e.AttemptedValue));

//                return s_implicitConversionFunc(new ValidationFailure(errors));
//            }
//        }

//        TResponse res = await next();
//        return res;
//    }
//}

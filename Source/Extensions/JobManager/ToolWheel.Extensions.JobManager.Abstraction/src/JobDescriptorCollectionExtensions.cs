using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ToolWheel.Extensions.JobManager.Configuration;

namespace ToolWheel;

public static class JobDescriptorCollectionExtensions
{
    public static JobDescriptionCollection AddType<T>(this JobDescriptionCollection collection)
    {
        return collection.AddType(typeof(T));
    }

    public static JobDescriptionCollection AddType(this JobDescriptionCollection collection, Type type)
    {
        var methods = from method in type.GetMethods()
                      let attribute = method.GetCustomAttribute<JobAttribute>()
                      where !method.IsSpecialName && attribute is not null
                      select method;

        foreach (var method in methods)
        {
            collection.AddMethod(method);
        }

        return collection;
    }

    public static void AddMethod<T>(this JobDescriptionCollection collection, Expression<Func<T, Delegate>> expr)
    {
        var method = ResolveMethodCall(expr.Body) ?? throw new ArgumentException("Expression does not represent a method call.");

        collection.AddMethod(method);
    }

    public static void AddMethod<T>(this JobDescriptionCollection collection, Expression<Func<T, Delegate>> expr, Action<JobDescriptionBuilder>? configure)
    {
        var method = ResolveMethodCall(expr.Body) ?? throw new ArgumentException("Expression does not represent a method call.");
        var jobDescription = JobDescriptionUtility.CreateJobDescription(method, m => JobIdResolver(collection, m));

        configure?.Invoke(new JobDescriptionBuilder(jobDescription));

        collection.Add(jobDescription);
    }

    public static void AddMethod(this JobDescriptionCollection collection, MethodInfo methodInfo)
    {
        if (collection == null) throw new ArgumentNullException(nameof(collection));
        if (methodInfo == null) throw new ArgumentNullException(nameof(methodInfo));

        var jobDescriptions = JobDescriptionUtility.CreateJobDescriptions(methodInfo, m => JobIdResolver(collection, m));

        collection.AddRange(jobDescriptions);
    }

    private static string JobIdResolver(JobDescriptionCollection collection, JobDescriptionIdInfo info)
    {
        if (info.Attribute is not null)
        {
            return info.Attribute.JobId ?? $"{info.Method.DeclaringType!.FullName}.{info.Method.Name}";
        }
        else
        {
            var count = collection.Count(m => m.Method == info.Method);

            return count == 0 ? MethodFullname(info.Method) : $"{MethodFullname(info.Method)}_{count}";
        }
    }

    private static string JobIdResolver(JobDescriptionCollection collection, MethodInfo method)
    {

        var count = collection.Count(m => m.Method == method);

        return count == 0 ? MethodFullname(method) : $"{MethodFullname(method)}_{count}";
    }

    private static string MethodFullname(MethodInfo method)
    {
        return $"{method.DeclaringType!.FullName}.{method.Name}";
    }

    private static MethodInfo? ResolveMethodCall(Expression expression)
    {
        var currentExpression = expression;

        while (true)
        {
            // Das muss gemacht werden, da der C#-Compiler ein Lambda-Ausdruck immer in eine LambdaExpression einbettet.
            if (currentExpression is LambdaExpression lambdaExpression)
            {
                // Da interessiert uns der Funktionsbody
                currentExpression = lambdaExpression.Body;
                continue;
            }

            // Der C#-Comiler interpretiert den Ausdruck m => m.JobMethod so das er eine Methode darum legt: Delegate.CreateDelegate( Action(() => m.JobMethod)). 
            // Der Compiler versucht das Delegate nun in eine Action zu konvertieren. Da Delegate nicht Typensicher ist.
            if (currentExpression is UnaryExpression unaryExpression)
            {
                // Aber hier interessiert nur der Operand, der hoffentlich der Aufruf Delegate.CreateDelegate ist.
                currentExpression = unaryExpression.Operand;
                continue;
            }

            // Wenn wir auf eine MethodCallExpression stoßen, ist es egal ob wir "glücklicherweise" die Zielmethode haben, oder der Umweg über die Delegate.CreateDelegate
            if (currentExpression is MethodCallExpression methodCallExpression)
            {
                // Mich interessiert nur das Object, das eine Konstante ist, die eine MethodInfo enthält.
                currentExpression = methodCallExpression.Object;
                // continue;
            }

            // So wenn wir hier angekommen sind und auf eine ConstantExpression gestoßen sind, müssen wir nur noch sicherstellen, dass der Type eine MethodInfo
            // ist und das Object zurückgeben.
            if (currentExpression is ConstantExpression constantExpression && constantExpression.Value is MethodInfo method)
            {
                return method;
            }

            return null;
        }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
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
            collection.AddMethod(method, null);
        }

        return collection;
    }

    public static void AddMethod<T>(this JobDescriptionCollection collection, Expression<Func<T, Delegate>> expr, Action<JobDescriptionBuilder>? configure = null)
    {
        var target = ResolveMethodCall(expr.Body) ?? throw new ArgumentException("Expression does not represent a method call.");

        AddMethod(collection, target, null, configure);
    }

    public static void AddMethod<T>(this JobDescriptionCollection collection, Delegate method, Action<JobDescriptionBuilder>? configure = null)
    {
        AddMethod(collection, method.Method, method.Target, configure);
    }

    public static void AddMethod(this JobDescriptionCollection collection, MethodInfo method, object? target, Action<JobDescriptionBuilder>? configure = null)
    {
        if (collection == null) throw new ArgumentNullException(nameof(collection));
        if (method == null) throw new ArgumentNullException(nameof(method));

        var builder = new JobDescriptionBuilder(method, target);
        var jobDescription = JobDescriptionConfigurationUtility.CreateJobDescription(method, target, configure, m => JobIdResolver(collection, m));

        collection.Add(jobDescription);
    }

    private static string JobIdResolver(JobDescriptionCollection collection, JobDescriptionIdInfo info)
    {
        if (info.Id is not null)
        {
            return info.Id ?? $"{info.Target.DeclaringType!.FullName}.{info.Target.Name}";
        }
        else
        {
            var count = collection.Count(m => m.Method == info.Target);

            return count == 0 ? MethodFullname(info.Target) : $"{MethodFullname(info.Target)}_{count}";
        }
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

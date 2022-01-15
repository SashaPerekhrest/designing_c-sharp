using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Reflection.Differentiation
{
   public static class Algebra
   {
      public static Expression<Func<double, double>> Differentiate(Expression<Func<double, double>> function)
         => Expression.Lambda<Func<double, double>>(Differentiate(function.Body), function.Parameters);

      private static Expression Differentiate(Expression expression)
      {
         switch (expression)
         {
            case MethodCallExpression e:
               return Differentiate(e);
            case BinaryExpression e:
               return Differentiate(e);
            case UnaryExpression e when e.NodeType == ExpressionType.Convert:
               return Differentiate(e.Operand);
            case MemberExpression e:
               return Differentiate(e.Expression);
            case ConstantExpression e:
               return Expression.Constant(0.0);
            case ParameterExpression e:
               return Expression.Constant(1.0);
            default: throw new ArgumentException();
         }
      }

      private static Expression Differentiate(MethodCallExpression expression)
      { 
         var message = $"Method \"{expression.Method.Name}()\" is not supported"; 
         if (expression.Method.DeclaringType != typeof(Math)) 
            throw new ArgumentException(message);
         
         var firsArg = expression.Arguments.First();

         switch (expression.Method.Name)
         {
            case "Sin":
               return Expression.Multiply(
                  Expression.Call(typeof(Math).GetMethod("Cos"), firsArg), 
                  Differentiate(firsArg));
            case "Cos":
               return Expression.Multiply(Expression.Negate(
                     Expression.Call(typeof(Math).GetMethod("Sin"), firsArg)), 
                  Differentiate(firsArg));
            default: throw  new ArgumentException(message);
         }
      }

      private static Expression Differentiate(BinaryExpression expression)
      {
         switch (expression.NodeType)
         {
            case ExpressionType.Add:
               return Expression.Add(Differentiate(expression.Left), 
                  Differentiate(expression.Right));
            case ExpressionType.Multiply:
               return Expression.Add(
                  Expression.Multiply(Differentiate(expression.Left), expression.Right),
                  Expression.Multiply(Differentiate(expression.Right), expression.Left));
            default: throw new ArgumentException();
         }
      }
   }
}


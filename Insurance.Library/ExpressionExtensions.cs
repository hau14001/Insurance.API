namespace System.Linq.Expressions
{
    public static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            if (left == null)
            {
                return right;
            }

            return Expression.Lambda<Func<T, bool>>(Expression.And(new SwapVisitor(left.Parameters[0], right.Parameters[0]).Visit(left.Body), right.Body), right.Parameters);
        }
    }

    internal class SwapVisitor : ExpressionVisitor
    {
        private readonly Expression from, to;

        public SwapVisitor(Expression from, Expression to)
        {
            this.from = from;
            this.to = to;
        }

        public override Expression Visit(Expression node)
        {
            return node == from ? to : base.Visit(node);
        }
    }
}

using System.Linq;

namespace ASOFT.Core.DataAccess
{
    public interface IRelationQuery<T, in TR1> : IQuery<T>
    {
        IQueryable<T> Spec(IQueryable<T> source, IQueryable<TR1> relation1);
    }

    public interface IRelationQuery<T, in TR1, in TR2> : IQuery<T>
    {
        IQueryable<T> Spec(IQueryable<T> source, IQueryable<TR1> relation1, IQueryable<TR2> relation2);
    }

    public interface IRelationQuery<T, in TR1, in TR2, in TR3> : IQuery<T>
    {
        IQueryable<T> Spec(IQueryable<T> source, IQueryable<TR1> relation1, IQueryable<TR2> relation2,
            IQueryable<TR3> relation3);
    }

    public interface IRelationQuery<T, in TR1, in TR2, in TR3, in TR4> : IQuery<T>
    {
        IQueryable<T> Spec(IQueryable<T> source, IQueryable<TR1> relation1, IQueryable<TR2> relation2,
            IQueryable<TR3> relation3, IQueryable<TR4> relation4);
    }

    public interface IRelationQuery<T, in TR1, in TR2, in TR3, in TR4, in TR5> : IQuery<T>
    {
        IQueryable<T> Spec(IQueryable<T> source, IQueryable<TR1> relation1, IQueryable<TR2> relation2,
            IQueryable<TR3> relation3, IQueryable<TR4> relation4,
            IQueryable<TR5> relation5);
    }

    public interface IRelationQuery<T, in TR1, in TR2, in TR3, in TR4, in TR5, in TR6> : IQuery<T>
    {
        IQueryable<T> Spec(IQueryable<T> source, IQueryable<TR1> relation1, IQueryable<TR2> relation2,
            IQueryable<TR3> relation3, IQueryable<TR4> relation4,
            IQueryable<TR5> relation5, IQueryable<TR6> relation6);
    }

    public interface
        IRelationQuery<T, in TR1, in TR2, in TR3, in TR4, in TR5, in TR6, in TR7> : IQuery<T>
    {
        IQueryable<T> Spec(IQueryable<T> source, IQueryable<TR1> relation1, IQueryable<TR2> relation2,
            IQueryable<TR3> relation3, IQueryable<TR4> relation4,
            IQueryable<TR5> relation5, IQueryable<TR6> relation6, IQueryable<TR7> relation7);
    }

    public interface
        IRelationQuery<T, in TR1, in TR2, in TR3, in TR4, in TR5, in TR6, in TR7, in TR8> : IQuery<T>
    {
        IQueryable<T> Spec(IQueryable<T> source, IQueryable<TR1> relation1, IQueryable<TR2> relation2,
            IQueryable<TR3> relation3, IQueryable<TR4> relation4,
            IQueryable<TR5> relation5, IQueryable<TR6> relation6, IQueryable<TR7> relation7, IQueryable<TR8> relation8);
    }
}
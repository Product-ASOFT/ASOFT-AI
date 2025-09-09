using System.Linq;

namespace ASOFT.Core.DataAccess
{
    public abstract class RelationQuery<T, TR1> : BaseQuery<T>, IRelationQuery<T, TR1>
    {
        public abstract IQueryable<T> Spec(IQueryable<T> source, IQueryable<TR1> relation1);
    }

    public abstract class RelationQuery<T, TR1, TR2> : BaseQuery<T>, IRelationQuery<T, TR1, TR2>
    {
        public abstract IQueryable<T> Spec(IQueryable<T> source, IQueryable<TR1> relation1, IQueryable<TR2> relation2);
    }

    public abstract class RelationQuery<T, TR1, TR2, TR3> : BaseQuery<T>,
        IRelationQuery<T, TR1, TR2, TR3>
    {
        public abstract IQueryable<T> Spec(IQueryable<T> source, IQueryable<TR1> relation1, IQueryable<TR2> relation2,
            IQueryable<TR3> relation3);
    }

    public abstract class RelationQuery<T, TR1, TR2, TR3, TR4> : BaseQuery<T>,
        IRelationQuery<T, TR1, TR2, TR3, TR4>
    {
        public abstract IQueryable<T> Spec(IQueryable<T> source, IQueryable<TR1> relation1, IQueryable<TR2> relation2,
            IQueryable<TR3> relation3, IQueryable<TR4> relation4);
    }

    public abstract class RelationQuery<T, TR1, TR2, TR3, TR4, TR5> : BaseQuery<T>,
        IRelationQuery<T, TR1, TR2, TR3, TR4, TR5>
    {
        public abstract IQueryable<T> Spec(IQueryable<T> source, IQueryable<TR1> relation1, IQueryable<TR2> relation2,
            IQueryable<TR3> relation3, IQueryable<TR4> relation4,
            IQueryable<TR5> relation5);
    }

    public abstract class RelationQuery<T, TR1, TR2, TR3, TR4, TR5, TR6> : BaseQuery<T>,
        IRelationQuery<T, TR1, TR2, TR3, TR4, TR5, TR6>
    {
        public abstract IQueryable<T> Spec(IQueryable<T> source, IQueryable<TR1> relation1, IQueryable<TR2> relation2,
            IQueryable<TR3> relation3, IQueryable<TR4> relation4,
            IQueryable<TR5> relation5, IQueryable<TR6> relation6);
    }

    public abstract class RelationQuery<T, TR1, TR2, TR3, TR4, TR5, TR6, TR7> : BaseQuery<T>,
        IRelationQuery<T, TR1, TR2, TR3, TR4, TR5, TR6, TR7>
    {
        public abstract IQueryable<T> Spec(IQueryable<T> source, IQueryable<TR1> relation1, IQueryable<TR2> relation2,
            IQueryable<TR3> relation3, IQueryable<TR4> relation4,
            IQueryable<TR5> relation5, IQueryable<TR6> relation6, IQueryable<TR7> relation7);
    }

    public abstract class RelationQuery<T, TR1, TR2, TR3, TR4, TR5, TR6, TR7, TR8> : BaseQuery<T>,
        IRelationQuery<T, TR1, TR2, TR3, TR4, TR5, TR6, TR7, TR8>
    {
        public abstract IQueryable<T> Spec(IQueryable<T> source, IQueryable<TR1> relation1, IQueryable<TR2> relation2,
            IQueryable<TR3> relation3, IQueryable<TR4> relation4,
            IQueryable<TR5> relation5, IQueryable<TR6> relation6, IQueryable<TR7> relation7, IQueryable<TR8> relation8);
    }
}
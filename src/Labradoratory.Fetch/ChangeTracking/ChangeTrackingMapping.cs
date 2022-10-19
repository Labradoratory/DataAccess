using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Labradoratory.Fetch.ChangeTracking
{
    // TODO: Needs more work.  Maybe in the future.

    ///// <summary>
    ///// Defines helpful mapping between generic collections and change tracking collections.
    ///// </summary>
    ///// <seealso cref="AutoMapper.Profile" />
    //public class ChangeTrackingMapping : Profile
    //{
    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="ChangeTrackingMapping"/> class.
    //    /// </summary>
    //    public ChangeTrackingMapping()
    //    {
    //        CreateMap(typeof(List<>), typeof(ChangeTrackingCollection<>))
    //            .ConvertUsing<CollectionToChangeTrackingConverter>(); 
    //        CreateMap(typeof(IList<>), typeof(ChangeTrackingCollection<>))
    //            .ConvertUsing<CollectionToChangeTrackingConverter>();
    //        CreateMap(typeof(Collection<>), typeof(ChangeTrackingCollection<>))
    //            .ConvertUsing<CollectionToChangeTrackingConverter>();
    //        CreateMap(typeof(ICollection<>), typeof(ChangeTrackingCollection<>))
    //            .ConvertUsing<CollectionToChangeTrackingConverter>();

    //        CreateMap(typeof(Dictionary<,>), typeof(ChangeTrackingDictionary<,>));            
    //        CreateMap(typeof(IDictionary<,>), typeof(ChangeTrackingDictionary<,>));
    //    }
    //}

    ///// <summary>
    ///// Converts an <see cref="ICollection{T}"/> into a <see cref="ChangeTrackingCollection{T}"/>
    ///// </summary>
    ///// <seealso cref="ITypeConverter{Object, Object}" />
    //public class CollectionToChangeTrackingConverter : ITypeConverter<object, object>
    //{
    //    private static MethodInfo _convertMethod;
    //    private static MethodInfo GetConvertMethod()
    //    {
    //        return _convertMethod ?? (_convertMethod = typeof(CollectionToChangeTrackingConverter).GetMethod("ConvertSpecific"));
    //    }

    //    /// <inheritdoc/>
    //    public object Convert(object source, object destination, ResolutionContext context)
    //    {
    //        var methodInfo = GetConvertMethod();
    //        var method = methodInfo.MakeGenericMethod(source.GetType().GenericTypeArguments[0]);
    //        return method.Invoke(this, new object[] { source, destination, context });
    //    }

    //    /// <summary>
    //    /// Converts the specified source to the destination type.
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <param name="source">The source.</param>
    //    /// <param name="destination">The destination.</param>
    //    /// <param name="context">The context.</param>
    //    /// <returns></returns>
    //    public ChangeTrackingCollection<T> ConvertSpecific<T>(ICollection<T> source, ChangeTrackingCollection<T> destination, ResolutionContext context)
    //    {
    //        var allowUpdate = typeof(T).GetInterfaces().Contains(typeof(ITracksChanges));
    //        var destinationItems = destination.ToList();
    //        var addItems = new List<T>();
    //        foreach (var sourceItem in source)
    //        {
    //            var di = destinationItems.IndexOf(sourceItem);
    //            if (di < 0)
    //            {
    //                addItems.Add(sourceItem);
    //                continue;
    //            }

    //            var item = destinationItems[di];
    //            destinationItems.RemoveAt(di);
    //            if (allowUpdate)
    //            {

    //            }
    //        }

    //        // These don't exist in the source, so remove them.
    //        foreach(var destinationItem in destinationItems)
    //        {
    //            destination.Remove(destinationItem);
    //        }
    //        // Add the new items.
    //        foreach(var sourceItem in addItems)
    //        {
    //            destinationItems.Add(sourceItem);
    //        }

    //        return destination;
    //    }
    //}
}

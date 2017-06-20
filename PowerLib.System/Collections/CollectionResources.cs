using System;
using System.Globalization;
using PowerLib.System.Resources;

namespace PowerLib.System.Collections
{
	internal sealed class CollectionResources : EnumTypeResourceAccessor<CollectionMessage, CollectionResources>
	{
    private static Lazy<CollectionResources> _default = new Lazy<CollectionResources>(() => new CollectionResources());

    private CollectionResources()
      : base(t => t.ToString())
    {
    }

    internal CollectionResources(CultureInfo ci)
      : base(t => t.ToString(), ci)
    {
    }

    public static CollectionResources Default
    {
      get { return _default.Value; }
    }
	}

	internal enum CollectionMessage
	{
		CollectionElementError,
		EnumeratorPositionAfterLast,
		EnumeratorPositionBeforeFirst,
		InternalCollectionDoesNotSupportCapacity,
		InternalCollectionDoesNotSupportStamp,
		InternalCollectionHasFixedLayout,
		InternalCollectionHasFixedSize,
		InternalCollectionHasReadOnlyValue,
		InternalCollectionIsEmpty,
		InternalCollectionIsFixed,
		InternalCollectionIsReadOnly,
		InternalCollectionNodeIsLeaf,
		InternalCollectionSlotIsNotEmpty,
		InternalCollectionWasModified,
		NoElementMatchedInPredicate,
		OneMoreElementMatchedInPredicate,
		SourceCollectionHasOneMoreElements,
		SourceCollectionIsEmpty,
	}
}

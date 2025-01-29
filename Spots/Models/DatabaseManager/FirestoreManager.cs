using Plugin.Firebase.Firestore;

namespace Spots
{
    public static class FirestoreManager
    {
        private const int MAXIMUM_RETRIES = 3;
        public static async Task<T?> GetDocumentData<T>(string Collection, string DocumentID)
        {
            T? retval = default;
            int retries = 0;

            while (retval == null && retries < MAXIMUM_RETRIES)
            {
                try
                {
                    IDocumentReference docRef = CrossFirebaseFirestore.Current.GetCollection(Collection).GetDocument(DocumentID);
                    IDocumentSnapshot<T> docSnap = await docRef.GetDocumentSnapshotAsync<T>();
                    retval = docSnap.Data;
                }
                catch (Exception ex)
                {
                    retval = default;
                    retries++;
                }
            }

            return retval;
        }

        public static async Task<List<T>> QueryFiltered<T>(string collection,
            int? maxItems = null,
            string? filters_orderBy = null,
            Dictionary<string, object>? filters_EqualsTo = null,
            Dictionary<string, object>? filters_ArrayContainsSingle = null,
            string? lastItemQueriedID = null)
        {
            List<T>? retVal = [];
            int globalRetries = 0;

            while (globalRetries < 5)
            {
                try
                {
                    IQuery query = CrossFirebaseFirestore.Current.GetCollection(collection);

                    if(maxItems != null)
                    {
                        query = query.LimitedTo(maxItems.Value);
                    }

                    foreach (var equalsFilter in filters_EqualsTo ?? [])
                    {
                        query = query.WhereEqualsTo(equalsFilter.Key, equalsFilter.Value);
                    }

                    if (filters_ArrayContainsSingle != null)
                    {
                        if(filters_ArrayContainsSingle.Count != 1) // Required condition as there can only be 1 such filter
                        {
                            throw new Exception("Query with multiple ArrayContainsSingle statement attempted.");
                        }

                        foreach (var arrayContainsSingleFilter in filters_ArrayContainsSingle)
                        {
                            query.WhereArrayContains(arrayContainsSingleFilter.Key, arrayContainsSingleFilter.Value);
                        }
                    }

                    if (filters_orderBy != null)
                    {
                        query = query.OrderBy(filters_orderBy);
                    }

                    int retries = 0;
                    while (lastItemQueriedID != null)
                    {
                        IDocumentSnapshot<T>? documentSnapshot = null;
                        try
                        {
                            documentSnapshot = await CrossFirebaseFirestore.Current.GetCollection(collection).GetDocument(lastItemQueriedID).GetDocumentSnapshotAsync<T>();

                            query = query.StartingAfter(documentSnapshot);
                        }
                        catch (Exception ex)
                        {
                            retries++;
                            if (retries > 5)
                            {
                                retVal = null;
                            }
                        }
                    }

                    if (retVal != null)
                    {
                        IQuerySnapshot<T> querySnapshot = await query.GetDocumentsAsync<T>();
                        foreach (var document in querySnapshot.Documents)
                        {
                            if (document != null)
                            {
                                retVal.Add(document.Data);
                            }
                        }

                        return retVal;
                    }

                    throw new Exception("MaxDBRetriesReached");
                }
                catch (Exception ex)
                {
                    globalRetries++;
                }
            }

            return retVal?? [];
        }

        public static async Task<List<T>> QueryFiltered<T>(string collection,
            int? maxItems = null,
            string? filters_orderBy = null,
            Dictionary<string, object>? filters_EqualsTo = null,
            Dictionary<string, object[]>? filters_ArrayContainsAny = null,
            string? lastItemQueriedID = null)
        {
            List<T>? retVal = [];
            int globalRetries = 0;

            while (globalRetries < 5)
            {
                try
                {
                    IQuery query = CrossFirebaseFirestore.Current.GetCollection(collection);

                    if (maxItems != null)
                    {
                        query = query.LimitedTo(maxItems.Value);
                    }

                    foreach (var equalsFilter in filters_EqualsTo ?? [])
                    {
                        query = query.WhereEqualsTo(equalsFilter.Key, equalsFilter.Value);
                    }

                    if (filters_ArrayContainsAny != null)
                    {
                        if (filters_ArrayContainsAny.Count != 1) // Required condition as there can only be 1 such filter
                        {
                            throw new Exception("Query with multiple ArrayContainsAny statement attempted.");
                        }

                        foreach (var arrayContainsAnyFilter in filters_ArrayContainsAny)
                        {
                            query.WhereArrayContainsAny(arrayContainsAnyFilter.Key, arrayContainsAnyFilter.Value);
                        }
                    }

                    if (filters_orderBy != null)
                    {
                        query = query.OrderBy(filters_orderBy);
                    }

                    int retries = 0;
                    while (lastItemQueriedID != null)
                    {
                        IDocumentSnapshot<T>? documentSnapshot = null;
                        try
                        {
                            documentSnapshot = await CrossFirebaseFirestore.Current.GetCollection(collection).GetDocument(lastItemQueriedID).GetDocumentSnapshotAsync<T>();

                            query = query.StartingAfter(documentSnapshot);
                        }
                        catch (Exception ex)
                        {
                            retries++;
                            if (retries > 5)
                            {
                                retVal = null;
                            }
                        }
                    }

                    if (retVal != null)
                    {
                        IQuerySnapshot<T> querySnapshot = await query.GetDocumentsAsync<T>();
                        foreach (var document in querySnapshot.Documents)
                        {
                            if (document != null)
                            {
                                retVal.Add(document.Data);
                            }
                        }

                        return retVal;
                    }

                    throw new Exception("MaxDBRetriesReached");
                }
                catch (Exception ex)
                {
                    globalRetries++;
                }
            }

            return retVal ?? [];
        }
    }
}

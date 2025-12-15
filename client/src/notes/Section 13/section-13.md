# Paging Sorting and Filering
1: How to Implement pagination on the API and the Client
2: Deferred Execution using the IQueryable
3: How to implement filtering and sorting both on the client and API side
4: Using the Action Filters.
5: Adding a Timeago Pipe
6: Implement caching in the client for the paginated resources.

# Pagination
1: Helps to avoid the performance problems
2: Parameters are passed by the query string
3: Page size should be limited
4: We should always page results

# Deferred Execution
What we mean by the deferred execution is that we build up a tree of expression inside the entity framework for our query and we store that as an IQueryable of type whatever, our query can contain many claused we can have the where, order by, and for the pagination we use Take() and Skip(). #

var query = context.Users
    .Where(x => x.Gender == gender)
    .OrderBy(x => x.UserName)
    .Take(5)
    .Skip(5)

and what we store inside this query is an IQueryable, now nothing is executed on the database side at this point.

var query = context.Users
    .Where(x => x.Gender == gender)
    .OrderBy(x => x.UserName)
    .Take(5)
    .Skip(5)
    .AsQueryable()

and we can explicity say that we want this as the IQueryable, but when we add this where clause and these other clauses inside here then we are effectively building the query and it is gonna be the type of IQueryable. Now nothing is executed on the database side yet, when the query is executed when we execute a ToListAsync or ToArrayAsync() or ToDictionaryAsync()

query.ToListAsync();
query.ToArrayAsync();
query.ToDictionaryAsync();
query.Count();

these are the points when it goes to the database.

# Singleton query
there is another way of execution of the query and that is the singletion query, and the example of that is when we need to count the number of records.

So when we are going to implement the pagination there will be two queries that will be going to the database because in order to make up the pagination response we need to let the user know how many records are there in the total based on that particular query.
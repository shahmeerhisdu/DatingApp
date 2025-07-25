# What are observeables?
these are available since the angular version 2, and these are new standard for managing async data included in ES7
observables are lazy collections of multiple values over time. they give us a way to observe or listen to multiple values over time.
You can think of the observeable as the newsletter
    only subscribers of the newsletter receive the newsletter 
    if no one subscribe the newsletter it probably would not be printed 
so observeables are lazy effectively and if we want to listen to an observable and get the values over the time then we have to subscribe to them, and if we subscribe to something then obviously that emplies that if you wanna stop listening to it you need to unsubscribe from it.

# Promise Vs Observables
In Javascript we have the option to use the promise which is also used in the aysncronus code or in angular we can use the observables we have that choice 

=> Promise provide a single future value, its a promise that a value will be provided in the future
=> Promises are not lazy, it will always gonna return something.
=> Can not be cancelled, if we have asked for something it is going to be delivered even if something is really big it will complete or will give us the error.

=> Observeables Emit multiple values over time, so it is very good for streaming data.
=> Observeables are lazy, it will do nothing if no body subscribes to it.
=> We can cancel observables
=> Can use with map, filter, reduce, and other operators for transforming the data and to do this we use a library called rxjs. 

if we talk about the observeables in angular then typically we will be using our HTTP Client in angular service thats the appropriate place to use that, and when we make an HTTP request with the angular HTTP client then this will always return an observeable and then we will attept to consume that inside our angular component 

incase of HTTP request that is subscribed we don't need to worry about unsubscribing it, because the HTTP request always completes.

we can also use the toPromise() directly if we want members as it is, like this
getMembers(){
    return this.http.get(api/users).toPromise()
}

# Aysnc Pipe
we also have something called as Async Pipe and we can use this inside our template and this will allow us to automatically subscribe and unsubscribe from the observeable without us needing to do so in our component.
<li *ngFor = let members of service.getMembers() | aysnc> {{member.userName}}</>

# Signals

There is the third choice we have but that is not to deal with the asynchronous code that is angular signals 
Now, angular traditionally has got a reputation from developers as being somewhat tricky to learn because of the nature of observeables. Because of RxJS library that we have to use when working with observeables. It can be perceived as slightly tricky to learn because of those things, but since angular 16, angular instroduced something called angular signals. Now these are not used with asynchronous operations. We still need to use the observeables with that. So things like HTTP requests where we make an HTTP request and then we have to wait for something to come back, well that's still going to be using the observeables and we will still continue to use them.

But in pervious iteration of this course, we also needed to use observeables when we wanted to notify components, other components in our application when something changes , even if that's not an asynchronous piece of code that we are using.

So to address that, angular introduced signals and signal is a wrapper around a value that notifies interested consumers when that value changes. Signal can contain any value, from primitives to complex data structures.

const count = signal(0)
//signals are getter functions - calling them reads their value.
console.log("The count is : "+ count());


//set a new value
count.set(3)

//update a value
count.update(value => value + 2)

Signals are great for 
state management 
they are good in performance they are more efficient than observeables in terms of change detection as they work by tracking the dependencies and only reevaluating when these dependencies change, and this can lead to less unnecessary updates and therefore better performance and have less boiler plate than observeables.
they are predictable as they are synchronous
they are also well integrated with the angular components.
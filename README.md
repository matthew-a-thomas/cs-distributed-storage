# cs-distributed-storage
Fountain Code-based distributed data storage for C# .Net Core.

Based on the [Random Subset Fountain Code](https://github.com/matthew-a-thomas/cs-fountain-codes#random-subset).

See the [ConsoleApp](https://github.com/matthew-a-thomas/cs-distributed-storage/tree/master/ConsoleApp) project for examples.

## The big picture

**Original data** is contained in an array of bytes. A **manifest** and a **slice generator** are created from this array. Generated **slices** are accumulated by a **solver**, which uses the manifest and accumulated slices to reproduce the data once enough slices are collected.

The **manifest** contains a SHA256 hash of the data, the length of the data, and SHA256 hashes of individual slices of the data.

The **slice generator** produces new slices by randomly combining subsets of the original data's slices.

A **slice** is just a byte array that has been tagged with information about which of the original data's slices were used to produce it.

The **solver** performs [Gaussian Elimination](https://en.wikipedia.org/wiki/Gaussian_elimination) on accumulated slices to reproduce the original data, which is verified using the associated manifest.

## In practice

Say you decide to make a slice generator and manifest by splitting your data up into 10 slices. Naturally, anybody who wants to reproduce your data will need to collect at least 10 generated slices (sometimes more, but never fewer). And they'd use the manifest to guarantee that what they retrieved matches your original data.

But since you have a slice generator, you can generate more than 10 slices. In fact, you can generate up to `(2^10)-1` distinct slices, because that's how many different subsets of 10 things there are (excluding the subset that has nothing).

Suppose you generate 1000 slices and spread them around in many places. You will have produced 100 times as much data, but have made it that much more likely for your data to survive. All anyone needs to do is find any 10 (and maybe a few more) of these 1000 slices. It doesn't matter which ones they get; they just have to get 10 of them.

But you can also do that with a [Reed-Solomon code](https://en.wikipedia.org/wiki/Reed%E2%80%93Solomon_error_correction). However, what you can't do with Reed-Solomon is continue to produce slices. Suppose over time your slices are lost. If you want to create more using Reed-Solomon you have to create all of them all over, and if you want more than 1000 then you additionally have to throw out all existing slices because they won't be useful any longer.

This isn't a problem for `DistributedStorage`. You can use the slice generator to generate more slices, and it *just works*.

You don't even have to use the same generator to generate more slices. As long as the original data is available, any number of generators can be generating slices at the same time (with little chance of generating overlapping slices because they come from random subsets of the original data, and there are *tons* of those) and can even be in different places.

All of these things together make it much easier for someone to reproduce your original data, and make it much more likely that your data will survive.

## The mechanism's essence

As you can see above, you can choose what level of protection you'd like your data to have. If you want it to be more likely to survive, you generate an abundance of slices. If you don't need that strong of a guarantee, you generate fewer slices.

Unfortunately, there's a little bit of overhead. As many slices will be required as the number of ways the original data was split up before generating slices, but an additional number of slices beyond that are sometimes required.

This is because putting *random* combinations of something back together is a *probabilistic* process.

To illustrate this, if you split your data into 2 parts, then 3 distinct slices are possible (the first part, the second part, and the combination of the first and second parts), which means there are 9 possible ways to collect 2 slices. But 3 of those 9 possible ways are collections of duplicates (you could collect the first slice twice, the second slice twice, or the third slice twice). This means you have a 6/9=66% chance of being able to reproduce the data after collecting only 2 slices.

You can imagine how collecting more slices would increase your chances of being able to get the data back. That number of additional slices is the overhead.

To be a little more formal, given data that has been split into *k* parts, then *n* slices are needed to get the data back with probability *p*, and *n* is at least as large as *k*. The *overhead* is *n-k*.

You can imagine how *p* depends on both *k* and *n*. As *k* increases, there are more distinct slices available, so there's less chance of collecting duplicates, so *p* gets better. As *n* increases beyond *k*, the *overhead* increases, so *p* gets better again.

Interestingly, we can know beforehand what the worst *p* will be by only knowing the *overhead*. This means we can know that the probability of getting data back will be at least a certain amount for a given *overhead*.

This means you can choose whatever *p* you'd like, and can guarantee it by ensuring there's always at least a given *overhead*.

In the future I'll try to lay out the mathematics behind this and to illustrate it with some graphs. I'll also try to provide some recommendations on *overhead* for a given *p*, which might take the form of a *p* parameter when constructing a solver.

## Projects in the .sln

The [`DistributedStorage.sln`](https://github.com/matthew-a-thomas/cs-distributed-storage/blob/master/DistributedStorage.sln) file has the following projects.

### ConsoleApp

This is a console program that lets you fiddle with distributed storage on files so that you get a feel for things.

### DistributedStorage

This is the core .dll. It exposes a few interfaces and extension methods, and a [`DistributedStorage`](https://github.com/matthew-a-thomas/cs-distributed-storage/blob/master/DistributedStorage/DistributedStorage.cs) class which provides implementations of the interfaces.

### DistributedStorageTests

This contains unit tests for the `DistributedStorage` .dll.

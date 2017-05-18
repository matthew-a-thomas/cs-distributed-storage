# cs-distributed-storage
Fountain Code-based distributed data storage for C# .Net Core.

Based on the [Random Subset Fountain Code](https://github.com/matthew-a-thomas/cs-fountain-codes#random-subset).

See the [ConsoleApp](https://github.com/matthew-a-thomas/cs-distributed-storage/tree/master/ConsoleApp) project for examples.

## Overview

**Original data** is contained in an array of bytes. A **manifest** and a **slice generator** are created from this array. Generated **slices** are accumulated by a **solver**, which uses the manifest and accumulated slices to reproduce the data once enough slices are collected.

The **manifest** contains a SHA256 hash of the data, the length of the data, and SHA256 hashes of individual slices of the data.

The **slice generator** produces new slices by randomly combining subsets of the original data's slices.

A **slice** is just a byte array that has been tagged with information about which of the original data's slices were used to produce it.

The **solver** performs [Gaussian Elimination](https://en.wikipedia.org/wiki/Gaussian_elimination) on accumulated slices to reproduce the original data, which is verified using the associated manifest.

## Projects in the .sln

The [`DistributedStorage.sln`](https://github.com/matthew-a-thomas/cs-distributed-storage/blob/master/DistributedStorage.sln) file has the following projects.

### ConsoleApp

This is a console program that lets you fiddle with distributed storage on files so that you get a feel for possible uses.

### DistributedStorage

This is the core .dll. It exposes a few interfaces and extension methods, and a [`DistributedStorage`](https://github.com/matthew-a-thomas/cs-distributed-storage/blob/master/DistributedStorage/DistributedStorage.cs) class which provides implementations of the interfaces.

### DistributedStorageTests

This contains unit tests for the `DistributedStorage` .dll.

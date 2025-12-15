using Microsoft.EntityFrameworkCore.Metadata;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

//why this file?
//await db.Database.EnsureDeletedAsync();
//await db.Database.MigrateAsync();

//If tests run in parallel, one test can delete the DB while another is using it.

//Fix: disable parallelization for this integration test assembly
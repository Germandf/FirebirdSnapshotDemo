using Microsoft.EntityFrameworkCore;
using System.Data;

await using var setup = new TestDbContext();
await setup.Database.EnsureDeletedAsync();
await setup.Database.EnsureCreatedAsync();

await using var snapshotCtx = new TestDbContext();
await using var snapshotTx = await snapshotCtx.Database.BeginTransactionAsync(IsolationLevel.Snapshot);
var isolationCode = await snapshotCtx.Database.SqlQueryRaw<int>(
    @"SELECT t.MON$ISOLATION_MODE AS ""Value"" FROM MON$TRANSACTIONS t WHERE t.MON$TRANSACTION_ID = CURRENT_TRANSACTION")
    .SingleAsync();
Console.WriteLine($"Isolation code: {isolationCode} (1 = SNAPSHOT)");

var before = await snapshotCtx.TestEntities.CountAsync();
Console.WriteLine($"[SNAPSHOT] Initial count: {before}");

await using var writerCtx = new TestDbContext();
writerCtx.TestEntities.Add(new TestEntity { Name = "Inserted while snapshot is open" });
await writerCtx.SaveChangesAsync();
Console.WriteLine("[WRITER] Insert committed");

var during = await snapshotCtx.TestEntities.CountAsync();
Console.WriteLine($"[SNAPSHOT] Count after insert: {during}");

await snapshotTx.CommitAsync();
Console.WriteLine("[SNAPSHOT] Transaction committed");

await using var finalCtx = new TestDbContext();
var after = await finalCtx.TestEntities.CountAsync();
Console.WriteLine($"[FINAL] Count after snapshot closed: {after}");

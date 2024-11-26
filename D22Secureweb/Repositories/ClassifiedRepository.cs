using D22Secureweb.Model;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace D22Secureweb.Repositories
{
    public class ClassifiedRepository : IClassifiedRepository
    {
        private readonly IMongoCollection<Classified> _classifiedDoc;

        public ClassifiedRepository(IOptions<D22RestDatabase> d22RestDatabase)
        {

            var mongoClient = new MongoClient(d22RestDatabase.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(d22RestDatabase.Value.DatabaseName);

            _classifiedDoc = mongoDatabase.GetCollection<Classified>(d22RestDatabase.Value.CollectionName);

            //create an index for performance reasons
            //var indexKeysDefinition = Builders<Classified>.IndexKeys.Descending("Version");
            //_classifiedDoc.Indexes.CreateOneAsync(new CreateIndexModel<Classified>(indexKeysDefinition));

            var indexKeysDefinition = Builders<Classified>.IndexKeys.Combine(
                Builders<Classified>.IndexKeys.Descending("ClassifiedID"),
                Builders<Classified>.IndexKeys.Descending("Version"));                
            _classifiedDoc.Indexes.CreateOneAsync(new CreateIndexModel<Classified>(indexKeysDefinition));

            //in mongosh use:
            //db.COLLECTION_NAME.createIndex({KEY:1})

        }

        public async Task Add(Classified cData)
        {
            //Using query without builders 
            var lastCls = await _classifiedDoc.Find<Classified>(x => x.ClassifiedID == cData.ClassifiedID)
                                            .SortByDescending(x => x.Version)
                                            .Limit(1)
                                            .FirstOrDefaultAsync();

            if (lastCls != null)
            {
                cData.Version = lastCls.Version + 1;
            }

            await _classifiedDoc.InsertOneAsync(cData);
        }

        public async Task<bool> Delete(int classifiedID)
        {
            var res = await _classifiedDoc.DeleteOneAsync<Classified>(x => x.ClassifiedID == classifiedID);
            return res.IsAcknowledged;
        }

        public async Task<Classified?> Get(int classifiedID)
        {
            return await _classifiedDoc.Find<Classified>(x => x.ClassifiedID == classifiedID)
                                         .SortByDescending(x => x.Version)
                                         .Limit(1)
                                         .FirstOrDefaultAsync();
        }

        public async Task<List<Classified>> GetAll()
        {
            return await _classifiedDoc.Find(x => true).ToListAsync(); ;
        }
    }
}

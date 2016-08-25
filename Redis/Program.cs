using ServiceStack.Redis;
using ServiceStack.Redis.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Redis
{
    public class Program
    {
        static void Main(string[] args)
        {
            string host = "localhost";
            int port = 6379;
            string elementKey = "testKeyRedis";
            using (RedisClient redisClient = new RedisClient(host, port))
            {
                if(redisClient.Get<string>(elementKey)==null)
                {
                    redisClient.Set(elementKey, "some cached value");
                }

                Console.WriteLine("Item value is: {0}", redisClient.Get<string>(elementKey));

                IRedisTypedClient<Phone> phones = redisClient.As<Phone>();
                Phone phoneFive = phones.GetValue("5");
                if (phoneFive == null)
                {
                    phoneFive = new Redis.Phone
                    {
                        Id = 5,
                        Manufacturer = "Apple",
                        Model = "xxxx",
                        Owner = new Person
                        {
                            Id = 1,
                            Age = 90,
                            Name = "OldOne",
                            Profession = "teacher",
                            Surname = "SurName"
                        }
                    };
                }
                phones.SetValue(phoneFive.Id.ToString(), phoneFive);
                Console.WriteLine("Phone model is: {0},  Phone Owner Name is: {1}", phoneFive.Manufacturer, phoneFive.Owner.Name);
            }

            Console.ReadKey();
        }
    }
}

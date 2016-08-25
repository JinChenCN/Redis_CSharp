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

                IRedisList<Phone> mostSelling = phones.Lists["urn:phones:mostselling"];
                IRedisList<Phone> oldCollection = phones.Lists["urn:phones:oldCollection"];

                Person phoneOwner = new Person
                {
                    Id = 7,
                    Age = 90,
                    Name = "OldOne",
                    Profession = "teacher",
                    Surname = "SurName"
                };

                mostSelling.Add(new Phone
                {
                    Id = 5,
                    Manufacturer = "Apple",
                    Model = "54321",
                    Owner = phoneOwner
                });

                oldCollection.Add(new Phone
                {
                    Id = 8,
                    Manufacturer = "Moto",
                    Model = "111111",
                    Owner = phoneOwner
                });

                var upgradedPhone = new Phone
                {
                    Id = 5,
                    Manufacturer = "LG",
                    Model = "12345",
                    Owner = phoneOwner
                };
                
                mostSelling.Add(upgradedPhone);
                Console.WriteLine("Phones in mostSelling list:");
                foreach (Phone ph in mostSelling)
                {
                    Console.WriteLine(ph.Id);
                }

                Console.WriteLine("Phones in oldCollection list:");
                foreach (Phone ph in oldCollection)
                {
                    Console.WriteLine(ph.Id);
                }
                oldCollection.Remove(upgradedPhone);
                IEnumerable<Phone> LGPhones = mostSelling.Where(ph => ph.Manufacturer == "LG");
                foreach (Phone ph in LGPhones)
                {
                    Console.WriteLine("LG phone Id: {0}, LG phone Model: {1}", ph.Id, ph.Model);
                }

                Phone singleElement = oldCollection.FirstOrDefault(ph => ph.Id == 8);
                Console.WriteLine("singleElement phone Id: {0}, singleElement phone Model: {1}", singleElement.Id, singleElement.Model);

                phones.SetSequence(0);

                redisClient.Remove("urn:phones:mostselling");
                redisClient.Remove("urn:phones:oldCollection");

                Console.WriteLine("urn:phones:mostselling Count: {0}", phones.Lists["urn:phones:mostselling"].Count);

            }

            Console.ReadKey();
        }
    }
}

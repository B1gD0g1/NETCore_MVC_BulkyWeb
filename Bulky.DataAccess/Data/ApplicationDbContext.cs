using Microsoft.EntityFrameworkCore;
using Bulky.Models;
using Bulky.DataAccess.Data.TableConfiguration;

namespace Bulky.DataAccess.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
               
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ProductConfiguration());



            //Seed Data
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Action", DisplayOrder = 1},
                new Category { Id = 2, Name = "SciFi", DisplayOrder = 2},
                new Category { Id = 3, Name = "History", DisplayOrder = 3}
                );

            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    ProductId = 1,
                    Title = "西游记",
                    Author = "吴承恩",
                    Description = "主要讲述了孙悟空出世，并寻菩提祖师学艺及大闹天宫后，与猪八戒、沙僧和白龙马一同护送唐僧西天取经，于路上历经险阻，降妖除魔，渡过了九九八十一难，成功到达大雷音寺，向如来佛祖求得《三藏真经》，最后五圣成真的故事。",
                    ISBN = "SWD9999001",
                    ListPrice = 99,
                    Price = 90,
                    Price50 = 85,
                    Price100 = 80,
                    CategoryId = 3,
                    ImageUrl = ""
                },
                new Product
                {
                    ProductId = 2,
                    Title = "红楼梦",
                    Author = "曹雪芹",
                    Description = "书叙西方灵河岸上三生石畔的绛珠仙子，为了酬报神瑛侍者的灌溉之恩，要将毕生的泪水偿还，就随其下凡历劫。宝玉为神瑛侍者转世，林黛玉为绛珠仙子转世，这段姻缘称为“木石前盟”。",
                    ISBN = "CAW777777701",
                    ListPrice = 40,
                    Price = 30,
                    Price50 = 25,
                    Price100 = 20,
                    CategoryId = 3,
                    ImageUrl = ""
                },
                new Product
                {
                    ProductId = 3,
                    Title = "无人生还",
                    Author = "阿加莎·克里斯蒂",
                    Description = "八个素不相识的人受邀来到海岛黑人岛上。他们抵达后，接待他们的却只是管家罗杰斯夫妇俩。用晚餐的时候，餐厅里的留声机忽然响起，指控他们宾客以及管家夫妇这十人都曾犯有谋杀罪。众人正在惶恐之际，来宾之一忽然死亡，噩梦由此开始了。",
                    ISBN = "RITO5555501",
                    ListPrice = 55,
                    Price = 50,
                    Price50 = 40,
                    Price100 = 35,
                    CategoryId = 4,
                    ImageUrl = ""
                },
                new Product
                {
                    ProductId = 4,
                    Title = "福尔摩斯探案集",
                    Author = "柯南·道尔",
                    Description = "福尔摩斯自称是顾问侦探，当其他警探或私家侦探遇到困难时常向他求救。他头脑冷静、观察力敏锐、推理能力突出，善于通过观察与演绎推理和法学知识来解决问题。平常他都悠闲地在贝克街221号的B室里，抽着烟斗等待委托上门，一旦接到案子，他立刻会变成一匹追逐猎物的猎犬，开始锁定目标，将整个事件抽丝剥茧、层层过滤，直到最后真相大白。",
                    ISBN = "WS3333333301",
                    ListPrice = 70,
                    Price = 65,
                    Price50 = 60,
                    Price100 = 55,
                    CategoryId = 4,
                    ImageUrl = ""
                },
                new Product
                {
                    ProductId = 5,
                    Title = "肖申克的救赎",
                    Author = "斯蒂芬·埃德温·金",
                    Description = "银行家安迪被误判谋杀妻子及其情人而入狱后，如何不动声色、步步为营地谋划自救，最终成功越狱、重获自由的故事。",
                    ISBN = "SOTJ1111111101",
                    ListPrice = 30,
                    Price = 27,
                    Price50 = 25,
                    Price100 = 20,
                    CategoryId = 1,
                    ImageUrl = ""
                },
                new Product
                {
                    ProductId = 6,
                    Title = "鲁滨逊漂流记",
                    Author = "丹尼尔·笛福",
                    Description = "主人公鲁滨逊·克鲁索出生于一个中产阶级家庭，一生志在遨游四海的故事。一次在去非洲航海的途中遇到风暴，只身漂流到一个无人的荒岛上，开始了一段与世隔绝的生活。他凭着强韧的意志与不懈的努力，在荒岛上顽强地生存下来，在岛上生活了28年2个月零19天后，最终得以返回故乡。",
                    ISBN = "FOT000000001",
                    ListPrice = 25,
                    Price = 23,
                    Price50 = 22,
                    Price100 = 20,
                    CategoryId = 2,
                    ImageUrl = ""
                }
                );
        }
    }
}

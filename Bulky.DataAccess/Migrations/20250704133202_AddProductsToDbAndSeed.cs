using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Bulky.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddProductsToDbAndSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ISBN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Author = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ListPrice = table.Column<double>(type: "float", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Price50 = table.Column<double>(type: "float", nullable: false),
                    Price100 = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "Author", "Description", "ISBN", "ListPrice", "Price", "Price100", "Price50", "Title" },
                values: new object[,]
                {
                    { 1, "吴承恩", "主要讲述了孙悟空出世，并寻菩提祖师学艺及大闹天宫后，与猪八戒、沙僧和白龙马一同护送唐僧西天取经，于路上历经险阻，降妖除魔，渡过了九九八十一难，成功到达大雷音寺，向如来佛祖求得《三藏真经》，最后五圣成真的故事。", "SWD9999001", 99.0, 90.0, 80.0, 85.0, "西游记" },
                    { 2, "曹雪芹", "书叙西方灵河岸上三生石畔的绛珠仙子，为了酬报神瑛侍者的灌溉之恩，要将毕生的泪水偿还，就随其下凡历劫。宝玉为神瑛侍者转世，林黛玉为绛珠仙子转世，这段姻缘称为“木石前盟”。", "CAW777777701", 40.0, 30.0, 20.0, 25.0, "红楼梦" },
                    { 3, "阿加莎·克里斯蒂", "八个素不相识的人受邀来到海岛黑人岛上。他们抵达后，接待他们的却只是管家罗杰斯夫妇俩。用晚餐的时候，餐厅里的留声机忽然响起，指控他们宾客以及管家夫妇这十人都曾犯有谋杀罪。众人正在惶恐之际，来宾之一忽然死亡，噩梦由此开始了。", "RITO5555501", 55.0, 50.0, 35.0, 40.0, "无人生还" },
                    { 4, "柯南·道尔", "福尔摩斯自称是顾问侦探，当其他警探或私家侦探遇到困难时常向他求救。他头脑冷静、观察力敏锐、推理能力突出，善于通过观察与演绎推理和法学知识来解决问题。平常他都悠闲地在贝克街221号的B室里，抽着烟斗等待委托上门，一旦接到案子，他立刻会变成一匹追逐猎物的猎犬，开始锁定目标，将整个事件抽丝剥茧、层层过滤，直到最后真相大白。", "WS3333333301", 70.0, 65.0, 55.0, 60.0, "福尔摩斯探案集" },
                    { 5, "斯蒂芬·埃德温·金", "银行家安迪被误判谋杀妻子及其情人而入狱后，如何不动声色、步步为营地谋划自救，最终成功越狱、重获自由的故事。", "SOTJ1111111101", 30.0, 27.0, 20.0, 25.0, "肖申克的救赎" },
                    { 6, "丹尼尔·笛福", "主人公鲁滨逊·克鲁索出生于一个中产阶级家庭，一生志在遨游四海的故事。一次在去非洲航海的途中遇到风暴，只身漂流到一个无人的荒岛上，开始了一段与世隔绝的生活。他凭着强韧的意志与不懈的努力，在荒岛上顽强地生存下来，在岛上生活了28年2个月零19天后，最终得以返回故乡。", "FOT000000001", 25.0, 23.0, 20.0, 22.0, "鲁滨逊漂流记" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}

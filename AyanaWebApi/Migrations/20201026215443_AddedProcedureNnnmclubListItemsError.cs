using Microsoft.EntityFrameworkCore.Migrations;

namespace AyanaWebApi.Migrations
{
    public partial class AddedProcedureNnnmclubListItemsError : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sp = @"CREATE PROCEDURE [dbo].[NnmclubListItemsError]
                       	@Created date
                       AS
                       BEGIN
                        SET NOCOUNT ON;
                       	select A.*
                       	from NnmclubListItems A
                       	full outer join NnmclubItems B
                       	on A.Id = B.NnmclubListItemId
                       	where A.Id Is NULL or B.NnmclubListItemId is null and A.Created >= @Created
                       	order by A.Created desc
                       END";
            migrationBuilder.Sql(sp);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var sp = @"DROP PROCEDURE [dbo].[NnmclubListItemsError]";
            migrationBuilder.Sql(sp);
        }
    }
}

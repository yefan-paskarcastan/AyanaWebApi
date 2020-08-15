using Microsoft.EntityFrameworkCore.Migrations;

namespace AyanaWebApi.Migrations
{
    public partial class AddProcedureRutorListItemError : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sp = @"CREATE PROCEDURE [dbo].[RutorListItemsError]
                        @Created date
                       AS
                       BEGIN
                        SET NOCOUNT ON;
                       	select A.*
                       	from RutorListItems A
                       	full outer join RutorItems B
                       	on A.Id = B.RutorListItemId
                       	where A.Id Is NULL or B.RutorListItemId is null and A.Created >= @Created
                       	order by A.Created desc
                       END";
            migrationBuilder.Sql(sp);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var sp = @"DROP PROCEDURE [dbo].[RutorListItemsError]";
            migrationBuilder.Sql(sp);
        }
    }
}

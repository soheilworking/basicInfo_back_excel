using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ORG.BasicInfo.Data.Migrations
{
    /// <inheritdoc />
    public partial class FormsSyss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FilesFormsSyss",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdFormUser = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    FileSize = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    UploadDate = table.Column<long>(type: "bigint", nullable: false),
                    LUserCreate = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KeyFile = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilesFormsSyss", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FormRawLogSyss",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdUser = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdForm = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Timestamp = table.Column<long>(type: "bigint", nullable: false),
                    Ip = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormRawLogSyss", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FormRawSyss",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdCode = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: false),
                    ExpireDate = table.Column<long>(type: "bigint", nullable: false),
                    IsPublicForm = table.Column<bool>(type: "bit", nullable: false),
                    TCreate = table.Column<long>(type: "bigint", nullable: false),
                    LUserCreate = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TEdit = table.Column<long>(type: "bigint", nullable: false),
                    LUserEdit = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormRawSyss", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FormUserLogSyss",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdFormRaw = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdFormUser = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IdUser = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdUserRead = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ip = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StateAction = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormUserLogSyss", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FormUserSyss",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdFormRaw = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    IdUserRead = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdUser = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StateAction = table.Column<int>(type: "int", nullable: false),
                    TCreate = table.Column<long>(type: "bigint", nullable: false),
                    LUserCreate = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TEdit = table.Column<long>(type: "bigint", nullable: false),
                    LUserEdit = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormUserSyss", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PermissionFunds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdUser = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false),
                    IdFund = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionFunds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Mobile = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    FundName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    IsOrgUser = table.Column<bool>(type: "bit", nullable: false),
                    FundCode = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    UserRole = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false),
                    IdCodeNationalFund = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    NationalCodeUser = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    IpUser = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false),
                    Certificate = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false),
                    TCreate = table.Column<long>(type: "bigint", nullable: false),
                    TEdit = table.Column<long>(type: "bigint", nullable: false),
                    LUserCreate = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LUserEdit = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FilesRawSyss",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdFormRaw = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FileSize = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    UploadDate = table.Column<long>(type: "bigint", nullable: false),
                    LUserCreate = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KeyFile = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilesRawSyss", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FilesRawSyss_FormRawSyss_IdFormRaw",
                        column: x => x.IdFormRaw,
                        principalTable: "FormRawSyss",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FormRawRelatedUserSys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdForm = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdUser = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormRawRelatedUserSys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormRawRelatedUserSys_FormRawSyss_IdForm",
                        column: x => x.IdForm,
                        principalTable: "FormRawSyss",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FormRawRelatedUserSys_Users_IdUser",
                        column: x => x.IdUser,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FilesFormsSyss_Id",
                table: "FilesFormsSyss",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FilesRawSyss_Id",
                table: "FilesRawSyss",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FilesRawSyss_IdFormRaw",
                table: "FilesRawSyss",
                column: "IdFormRaw");

            migrationBuilder.CreateIndex(
                name: "IX_FilesRawSyss_LUserCreate",
                table: "FilesRawSyss",
                column: "LUserCreate");

            migrationBuilder.CreateIndex(
                name: "IX_FilesRawSyss_UploadDate",
                table: "FilesRawSyss",
                column: "UploadDate");

            migrationBuilder.CreateIndex(
                name: "IX_FormRawLogSyss_Id",
                table: "FormRawLogSyss",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FormRawRelatedUserSys_Id",
                table: "FormRawRelatedUserSys",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FormRawRelatedUserSys_IdForm",
                table: "FormRawRelatedUserSys",
                column: "IdForm");

            migrationBuilder.CreateIndex(
                name: "IX_FormRawRelatedUserSys_IdUser",
                table: "FormRawRelatedUserSys",
                column: "IdUser");

            migrationBuilder.CreateIndex(
                name: "IX_FormRawSyss_ExpireDate",
                table: "FormRawSyss",
                column: "ExpireDate");

            migrationBuilder.CreateIndex(
                name: "IX_FormRawSyss_Id",
                table: "FormRawSyss",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FormRawSyss_IdCode",
                table: "FormRawSyss",
                column: "IdCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FormRawSyss_TCreate",
                table: "FormRawSyss",
                column: "TCreate");

            migrationBuilder.CreateIndex(
                name: "IX_FormRawSyss_TEdit",
                table: "FormRawSyss",
                column: "TEdit");

            migrationBuilder.CreateIndex(
                name: "IX_FormRawSyss_Title",
                table: "FormRawSyss",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_FormUserLogSyss_Id",
                table: "FormUserLogSyss",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FormUserSyss_Id",
                table: "FormUserSyss",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PermissionFunds_IdUser",
                table: "PermissionFunds",
                column: "IdUser");

            migrationBuilder.CreateIndex(
                name: "IX_Users_FundCode",
                table: "Users",
                column: "FundCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Mobile",
                table: "Users",
                column: "Mobile",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FilesFormsSyss");

            migrationBuilder.DropTable(
                name: "FilesRawSyss");

            migrationBuilder.DropTable(
                name: "FormRawLogSyss");

            migrationBuilder.DropTable(
                name: "FormRawRelatedUserSys");

            migrationBuilder.DropTable(
                name: "FormUserLogSyss");

            migrationBuilder.DropTable(
                name: "FormUserSyss");

            migrationBuilder.DropTable(
                name: "PermissionFunds");

            migrationBuilder.DropTable(
                name: "FormRawSyss");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}

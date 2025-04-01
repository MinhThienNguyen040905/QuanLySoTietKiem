using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhanMemQuanLySTK.Migrations
{
    /// <inheritdoc />
    public partial class update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Interest_Rates",
                columns: table => new
                {
                    Interest_Rate_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Term = table.Column<int>(type: "int", nullable: false),
                    Rate = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interest_Rates", x => x.Interest_Rate_ID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Username = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fullname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Money = table.Column<long>(type: "bigint", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dob = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Avatar = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Identity_Card = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Username);
                });

            migrationBuilder.CreateTable(
                name: "Group_Savings_Accounts",
                columns: table => new
                {
                    Saving_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Creating_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Money = table.Column<long>(type: "bigint", nullable: false),
                    Target = table.Column<long>(type: "bigint", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Interest_Rate_ID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Group_Savings_Accounts", x => x.Saving_ID);
                    table.ForeignKey(
                        name: "FK_Group_Savings_Accounts_Interest_Rates_Interest_Rate_ID",
                        column: x => x.Interest_Rate_ID,
                        principalTable: "Interest_Rates",
                        principalColumn: "Interest_Rate_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Personal_Savings_Accounts",
                columns: table => new
                {
                    Saving_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Creating_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Money = table.Column<long>(type: "bigint", nullable: false),
                    Target = table.Column<long>(type: "bigint", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Interest_Rate_ID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personal_Savings_Accounts", x => x.Saving_ID);
                    table.ForeignKey(
                        name: "FK_Personal_Savings_Accounts_Interest_Rates_Interest_Rate_ID",
                        column: x => x.Interest_Rate_ID,
                        principalTable: "Interest_Rates",
                        principalColumn: "Interest_Rate_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Personal_Savings_Accounts_Users_Username",
                        column: x => x.Username,
                        principalTable: "Users",
                        principalColumn: "Username",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Group_Details",
                columns: table => new
                {
                    Saving_ID = table.Column<int>(type: "int", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Total_Money = table.Column<long>(type: "bigint", nullable: false),
                    Is_Owner = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Group_Details", x => new { x.Username, x.Saving_ID });
                    table.ForeignKey(
                        name: "FK_Group_Details_Group_Savings_Accounts_Saving_ID",
                        column: x => x.Saving_ID,
                        principalTable: "Group_Savings_Accounts",
                        principalColumn: "Saving_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Group_Details_Users_Username",
                        column: x => x.Username,
                        principalTable: "Users",
                        principalColumn: "Username");
                });

            migrationBuilder.CreateTable(
                name: "Group_Notifications",
                columns: table => new
                {
                    Group_Notification_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Money = table.Column<long>(type: "bigint", nullable: true),
                    Username_Sender = table.Column<string>(type: "nvarchar(256)", nullable: true),
                    Notification_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Saving_ID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Group_Notifications", x => x.Group_Notification_ID);
                    table.ForeignKey(
                        name: "FK_Group_Notifications_Group_Savings_Accounts_Saving_ID",
                        column: x => x.Saving_ID,
                        principalTable: "Group_Savings_Accounts",
                        principalColumn: "Saving_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Group_Notifications_Users_Username_Sender",
                        column: x => x.Username_Sender,
                        principalTable: "Users",
                        principalColumn: "Username");
                });

            migrationBuilder.CreateTable(
                name: "Group_Transactions_Information",
                columns: table => new
                {
                    Transaction_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Transaction_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Money = table.Column<long>(type: "bigint", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Saving_ID = table.Column<int>(type: "int", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Group_Transactions_Information", x => x.Transaction_ID);
                    table.ForeignKey(
                        name: "FK_Group_Transactions_Information_Group_Savings_Accounts_Saving_ID",
                        column: x => x.Saving_ID,
                        principalTable: "Group_Savings_Accounts",
                        principalColumn: "Saving_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Group_Transactions_Information_Users_Username",
                        column: x => x.Username,
                        principalTable: "Users",
                        principalColumn: "Username");
                });

            migrationBuilder.CreateTable(
                name: "Personal_Notifications",
                columns: table => new
                {
                    Personal_Notification_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Money_Rate = table.Column<long>(type: "bigint", nullable: false),
                    Is_Read = table.Column<bool>(type: "bit", nullable: false),
                    Notification_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Is_Deleted = table.Column<bool>(type: "bit", nullable: false),
                    Saving_ID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personal_Notifications", x => x.Personal_Notification_ID);
                    table.ForeignKey(
                        name: "FK_Personal_Notifications_Personal_Savings_Accounts_Saving_ID",
                        column: x => x.Saving_ID,
                        principalTable: "Personal_Savings_Accounts",
                        principalColumn: "Saving_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Personal_Transactions_Information",
                columns: table => new
                {
                    Transaction_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Transaction_Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Money = table.Column<long>(type: "bigint", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Saving_ID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personal_Transactions_Information", x => x.Transaction_ID);
                    table.ForeignKey(
                        name: "FK_Personal_Transactions_Information_Personal_Savings_Accounts_Saving_ID",
                        column: x => x.Saving_ID,
                        principalTable: "Personal_Savings_Accounts",
                        principalColumn: "Saving_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Group_Notifications_Details",
                columns: table => new
                {
                    Group_Notification_ID = table.Column<int>(type: "int", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Is_Deleted = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Group_Notifications_Details", x => new { x.Username, x.Group_Notification_ID });
                    table.ForeignKey(
                        name: "FK_Group_Notifications_Details_Group_Notifications_Group_Notification_ID",
                        column: x => x.Group_Notification_ID,
                        principalTable: "Group_Notifications",
                        principalColumn: "Group_Notification_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Group_Notifications_Details_Users_Username",
                        column: x => x.Username,
                        principalTable: "Users",
                        principalColumn: "Username",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Group_Details_Saving_ID",
                table: "Group_Details",
                column: "Saving_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Group_Notifications_Saving_ID",
                table: "Group_Notifications",
                column: "Saving_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Group_Notifications_Username_Sender",
                table: "Group_Notifications",
                column: "Username_Sender");

            migrationBuilder.CreateIndex(
                name: "IX_Group_Notifications_Details_Group_Notification_ID",
                table: "Group_Notifications_Details",
                column: "Group_Notification_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Group_Savings_Accounts_Interest_Rate_ID",
                table: "Group_Savings_Accounts",
                column: "Interest_Rate_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Group_Transactions_Information_Saving_ID",
                table: "Group_Transactions_Information",
                column: "Saving_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Group_Transactions_Information_Username",
                table: "Group_Transactions_Information",
                column: "Username");

            migrationBuilder.CreateIndex(
                name: "IX_Personal_Notifications_Saving_ID",
                table: "Personal_Notifications",
                column: "Saving_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Personal_Savings_Accounts_Interest_Rate_ID",
                table: "Personal_Savings_Accounts",
                column: "Interest_Rate_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Personal_Savings_Accounts_Username",
                table: "Personal_Savings_Accounts",
                column: "Username");

            migrationBuilder.CreateIndex(
                name: "IX_Personal_Transactions_Information_Saving_ID",
                table: "Personal_Transactions_Information",
                column: "Saving_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Group_Details");

            migrationBuilder.DropTable(
                name: "Group_Notifications_Details");

            migrationBuilder.DropTable(
                name: "Group_Transactions_Information");

            migrationBuilder.DropTable(
                name: "Personal_Notifications");

            migrationBuilder.DropTable(
                name: "Personal_Transactions_Information");

            migrationBuilder.DropTable(
                name: "Group_Notifications");

            migrationBuilder.DropTable(
                name: "Personal_Savings_Accounts");

            migrationBuilder.DropTable(
                name: "Group_Savings_Accounts");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Interest_Rates");
        }
    }
}

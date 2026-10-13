using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Township_API.Migrations
{
    /// <inheritdoc />
    public partial class codetypesupdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.RenameColumn(
            //    name: "Discriminator",
            //    table: "tblModuleData",
            //    newName: "ModuleType");

            migrationBuilder.RenameColumn(
                name: "uid",
                table: "[tblUser]",
                newName: "UID");

            migrationBuilder.RenameColumn(
                name: "phone",
                table: "[tblUser]",
                newName: "Phone");

            migrationBuilder.RenameColumn(
                name: "password",
                table: "[tblUser]",
                newName: "Password");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "[tblUser]",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "[tblUser]",
                newName: "UserName");

            migrationBuilder.CreateTable(
                name: "DependentLandowners",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PID = table.Column<int>(type: "int", nullable: false),
                    CSN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IDNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TagNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PANnumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PassportNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LicenseNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AadharCardId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VoterID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Firstname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MiddleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShortName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BloodGroup = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DOB = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MobileNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CardIssueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CardPrintingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LogicalDeleted = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DependLandOwnerIssueDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DependentLandowners", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DependentResident",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PID = table.Column<int>(type: "int", nullable: false),
                    CSN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IDNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TagNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PANnumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PassportNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LicenseNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ICEno = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AadharCardId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VoterID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MiddleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShortName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BloodGroup = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DOB = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EmailID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MobileNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LandLine = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Building = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FlatNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CardIssueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CardPrintingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RegistrationIssueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LogicalDeleted = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DependentResident", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DependentTenent",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PID = table.Column<int>(type: "int", nullable: false),
                    CSN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IDNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TagNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PANnumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PassportNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LicenseNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ICEno = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AadharCardId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VoterID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MiddleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShortName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BloodGroup = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DOB = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EmailID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MobileNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LandLine = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Building = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FlatNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CardIssueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CardPrintingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RegistrationIssueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Aggreement_From = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Aggreement_To = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LogicalDeleted = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DependentTenent", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Landowners",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CSN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IDNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TagNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PANnumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PassportNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LicenseNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ICEno = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AadharCardId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VoterID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MiddleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShortName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BloodGroup = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DOB = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EmailID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MobileNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LandLine = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Building = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FlatNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CardIssueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CardPrintingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LogicalDeleted = table.Column<int>(type: "int", nullable: false),
                    LandOwnerIssueDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Landowners", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PrimaryResident",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CSN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IDNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TagNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PANnumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PassportNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LicenseNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ICEno = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AadharCardId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VoterID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MiddleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShortName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BloodGroup = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DOB = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EmailID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MobileNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LandLine = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Building = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FlatNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CardIssueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CardPrintingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RegistrationIssueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LogicalDeleted = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrimaryResident", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PrimaryTenent",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RID = table.Column<int>(type: "int", nullable: false),
                    CSN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IDNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TagNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PANnumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PassportNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LicenseNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ICEno = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AadharCardId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VoterID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MiddleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShortName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BloodGroup = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DOB = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EmailID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MobileNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LandLine = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Building = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FlatNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TenentType = table.Column<int>(type: "int", nullable: false),
                    CardIssueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CardPrintingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RegistrationIssueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Aggreement_From = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Aggreement_To = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LogicalDeleted = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrimaryTenent", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DependentLandowners");

            migrationBuilder.DropTable(
                name: "DependentResident");

            migrationBuilder.DropTable(
                name: "DependentTenent");

            migrationBuilder.DropTable(
                name: "Landowners");

            migrationBuilder.DropTable(
                name: "PrimaryResident");

            migrationBuilder.DropTable(
                name: "PrimaryTenent");

            //migrationBuilder.RenameColumn(
            //    name: "ModuleType",
            //    table: "tblModuleData",
            //    newName: "Discriminator");

            migrationBuilder.RenameColumn(
                name: "UID",
                table: "[tblUser]",
                newName: "uid");

            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "[tblUser]",
                newName: "phone");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "[tblUser]",
                newName: "password");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "[tblUser]",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "[tblUser]",
                newName: "name");
        }
    }
}

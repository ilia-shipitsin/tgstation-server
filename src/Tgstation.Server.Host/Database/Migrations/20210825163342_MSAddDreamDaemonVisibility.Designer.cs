﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tgstation.Server.Host.Database.Migrations
{
	[DbContext(typeof(SqlServerDatabaseContext))]
	[Migration("20210825163342_MSAddDreamDaemonVisibility")]
	partial class MSAddDreamDaemonVisibility
	{
		/// <inheritdoc />
		protected override void BuildTargetModel(ModelBuilder modelBuilder)
		{
#pragma warning disable 612, 618
			modelBuilder
				.HasAnnotation("ProductVersion", "3.1.18")
				.HasAnnotation("Relational:MaxIdentifierLength", 128)
				.HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

			modelBuilder.Entity("Tgstation.Server.Host.Models.ChatBot", b =>
			{
				b.Property<long?>("Id")
					.ValueGeneratedOnAdd()
					.HasColumnType("bigint")
					.HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

				b.Property<int>("ChannelLimit")
					.HasColumnType("int");

				b.Property<string>("ConnectionString")
					.IsRequired()
					.HasColumnType("nvarchar(max)")
					.HasMaxLength(10000);

				b.Property<bool?>("Enabled")
					.HasColumnType("bit");

				b.Property<long>("InstanceId")
					.HasColumnType("bigint");

				b.Property<string>("Name")
					.IsRequired()
					.HasColumnType("nvarchar(100)")
					.HasMaxLength(100);

				b.Property<int>("Provider")
					.HasColumnType("int");

				b.Property<long>("ReconnectionInterval")
					.HasColumnType("bigint");

				b.HasKey("Id");

				b.HasIndex("InstanceId", "Name")
					.IsUnique();

				b.ToTable("ChatBots");
			});

			modelBuilder.Entity("Tgstation.Server.Host.Models.ChatChannel", b =>
			{
				b.Property<long>("Id")
					.ValueGeneratedOnAdd()
					.HasColumnType("bigint")
					.HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

				b.Property<long>("ChatSettingsId")
					.HasColumnType("bigint");

				b.Property<decimal?>("DiscordChannelId")
					.HasColumnType("decimal(20,0)");

				b.Property<string>("IrcChannel")
					.HasColumnType("nvarchar(100)")
					.HasMaxLength(100);

				b.Property<bool?>("IsAdminChannel")
					.IsRequired()
					.HasColumnType("bit");

				b.Property<bool?>("IsUpdatesChannel")
					.IsRequired()
					.HasColumnType("bit");

				b.Property<bool?>("IsWatchdogChannel")
					.IsRequired()
					.HasColumnType("bit");

				b.Property<string>("Tag")
					.HasColumnType("nvarchar(max)")
					.HasMaxLength(10000);

				b.HasKey("Id");

				b.HasIndex("ChatSettingsId", "DiscordChannelId")
					.IsUnique()
					.HasFilter("[DiscordChannelId] IS NOT NULL");

				b.HasIndex("ChatSettingsId", "IrcChannel")
					.IsUnique()
					.HasFilter("[IrcChannel] IS NOT NULL");

				b.ToTable("ChatChannels");
			});

			modelBuilder.Entity("Tgstation.Server.Host.Models.CompileJob", b =>
			{
				b.Property<long?>("Id")
					.ValueGeneratedOnAdd()
					.HasColumnType("bigint")
					.HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

				b.Property<string>("ByondVersion")
					.IsRequired()
					.HasColumnType("nvarchar(max)");

				b.Property<int?>("DMApiMajorVersion")
					.HasColumnType("int");

				b.Property<int?>("DMApiMinorVersion")
					.HasColumnType("int");

				b.Property<int?>("DMApiPatchVersion")
					.HasColumnType("int");

				b.Property<Guid?>("DirectoryName")
					.IsRequired()
					.HasColumnType("uniqueidentifier");

				b.Property<string>("DmeName")
					.IsRequired()
					.HasColumnType("nvarchar(max)");

				b.Property<int?>("GitHubDeploymentId")
					.HasColumnType("int");

				b.Property<long?>("GitHubRepoId")
					.HasColumnType("bigint");

				b.Property<long>("JobId")
					.HasColumnType("bigint");

				b.Property<int?>("MinimumSecurityLevel")
					.HasColumnType("int");

				b.Property<string>("Output")
					.IsRequired()
					.HasColumnType("nvarchar(max)");

				b.Property<string>("RepositoryOrigin")
					.HasColumnType("nvarchar(max)");

				b.Property<long>("RevisionInformationId")
					.HasColumnType("bigint");

				b.HasKey("Id");

				b.HasIndex("DirectoryName");

				b.HasIndex("JobId")
					.IsUnique();

				b.HasIndex("RevisionInformationId");

				b.ToTable("CompileJobs");
			});

			modelBuilder.Entity("Tgstation.Server.Host.Models.DreamDaemonSettings", b =>
			{
				b.Property<long>("Id")
					.ValueGeneratedOnAdd()
					.HasColumnType("bigint")
					.HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

				b.Property<string>("AdditionalParameters")
					.IsRequired()
					.HasColumnType("nvarchar(max)")
					.HasMaxLength(10000);

				b.Property<bool?>("AllowWebClient")
					.IsRequired()
					.HasColumnType("bit");

				b.Property<bool?>("AutoStart")
					.IsRequired()
					.HasColumnType("bit");

				b.Property<long>("HeartbeatSeconds")
					.HasColumnType("bigint");

				b.Property<long>("InstanceId")
					.HasColumnType("bigint");

				b.Property<int>("Port")
					.HasColumnType("int");

				b.Property<int>("SecurityLevel")
					.HasColumnType("int");

				b.Property<long>("StartupTimeout")
					.HasColumnType("bigint");

				b.Property<long>("TopicRequestTimeout")
					.HasColumnType("bigint");

				b.Property<int>("Visibility")
					.HasColumnType("int");

				b.HasKey("Id");

				b.HasIndex("InstanceId")
					.IsUnique();

				b.ToTable("DreamDaemonSettings");
			});

			modelBuilder.Entity("Tgstation.Server.Host.Models.DreamMakerSettings", b =>
			{
				b.Property<long>("Id")
					.ValueGeneratedOnAdd()
					.HasColumnType("bigint")
					.HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

				b.Property<int>("ApiValidationPort")
					.HasColumnType("int");

				b.Property<int>("ApiValidationSecurityLevel")
					.HasColumnType("int");

				b.Property<long>("InstanceId")
					.HasColumnType("bigint");

				b.Property<string>("ProjectName")
					.HasColumnType("nvarchar(max)")
					.HasMaxLength(10000);

				b.Property<bool?>("RequireDMApiValidation")
					.IsRequired()
					.HasColumnType("bit");

				b.HasKey("Id");

				b.HasIndex("InstanceId")
					.IsUnique();

				b.ToTable("DreamMakerSettings");
			});

			modelBuilder.Entity("Tgstation.Server.Host.Models.Instance", b =>
			{
				b.Property<long?>("Id")
					.ValueGeneratedOnAdd()
					.HasColumnType("bigint")
					.HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

				b.Property<long>("AutoUpdateInterval")
					.HasColumnType("bigint");

				b.Property<int>("ChatBotLimit")
					.HasColumnType("int");

				b.Property<int>("ConfigurationType")
					.HasColumnType("int");

				b.Property<string>("Name")
					.IsRequired()
					.HasColumnType("nvarchar(100)")
					.HasMaxLength(100);

				b.Property<bool?>("Online")
					.IsRequired()
					.HasColumnType("bit");

				b.Property<string>("Path")
					.IsRequired()
					.HasColumnType("nvarchar(450)");

				b.Property<string>("SwarmIdentifer")
					.HasColumnType("nvarchar(450)");

				b.HasKey("Id");

				b.HasIndex("Path", "SwarmIdentifer")
					.IsUnique()
					.HasFilter("[SwarmIdentifer] IS NOT NULL");

				b.ToTable("Instances");
			});

			modelBuilder.Entity("Tgstation.Server.Host.Models.InstancePermissionSet", b =>
			{
				b.Property<long>("Id")
					.ValueGeneratedOnAdd()
					.HasColumnType("bigint")
					.HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

				b.Property<decimal>("ByondRights")
					.HasColumnType("decimal(20,0)");

				b.Property<decimal>("ChatBotRights")
					.HasColumnType("decimal(20,0)");

				b.Property<decimal>("ConfigurationRights")
					.HasColumnType("decimal(20,0)");

				b.Property<decimal>("DreamDaemonRights")
					.HasColumnType("decimal(20,0)");

				b.Property<decimal>("DreamMakerRights")
					.HasColumnType("decimal(20,0)");

				b.Property<long>("InstanceId")
					.HasColumnType("bigint");

				b.Property<decimal>("InstancePermissionSetRights")
					.HasColumnType("decimal(20,0)");

				b.Property<long>("PermissionSetId")
					.HasColumnType("bigint");

				b.Property<decimal>("RepositoryRights")
					.HasColumnType("decimal(20,0)");

				b.HasKey("Id");

				b.HasIndex("InstanceId");

				b.HasIndex("PermissionSetId", "InstanceId")
					.IsUnique();

				b.ToTable("InstancePermissionSets");
			});

			modelBuilder.Entity("Tgstation.Server.Host.Models.Job", b =>
			{
				b.Property<long?>("Id")
					.ValueGeneratedOnAdd()
					.HasColumnType("bigint")
					.HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

				b.Property<decimal?>("CancelRight")
					.HasColumnType("decimal(20,0)");

				b.Property<decimal?>("CancelRightsType")
					.HasColumnType("decimal(20,0)");

				b.Property<bool?>("Cancelled")
					.IsRequired()
					.HasColumnType("bit");

				b.Property<long?>("CancelledById")
					.HasColumnType("bigint");

				b.Property<string>("Description")
					.IsRequired()
					.HasColumnType("nvarchar(max)");

				b.Property<long?>("ErrorCode")
					.HasColumnType("bigint");

				b.Property<string>("ExceptionDetails")
					.HasColumnType("nvarchar(max)");

				b.Property<long>("InstanceId")
					.HasColumnType("bigint");

				b.Property<DateTimeOffset?>("StartedAt")
					.IsRequired()
					.HasColumnType("datetimeoffset");

				b.Property<long>("StartedById")
					.HasColumnType("bigint");

				b.Property<DateTimeOffset?>("StoppedAt")
					.HasColumnType("datetimeoffset");

				b.HasKey("Id");

				b.HasIndex("CancelledById");

				b.HasIndex("InstanceId");

				b.HasIndex("StartedById");

				b.ToTable("Jobs");
			});

			modelBuilder.Entity("Tgstation.Server.Host.Models.OAuthConnection", b =>
			{
				b.Property<long>("Id")
					.ValueGeneratedOnAdd()
					.HasColumnType("bigint")
					.HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

				b.Property<string>("ExternalUserId")
					.IsRequired()
					.HasColumnType("nvarchar(100)")
					.HasMaxLength(100);

				b.Property<int>("Provider")
					.HasColumnType("int");

				b.Property<long?>("UserId")
					.HasColumnType("bigint");

				b.HasKey("Id");

				b.HasIndex("UserId");

				b.HasIndex("Provider", "ExternalUserId")
					.IsUnique();

				b.ToTable("OAuthConnections");
			});

			modelBuilder.Entity("Tgstation.Server.Host.Models.PermissionSet", b =>
			{
				b.Property<long?>("Id")
					.ValueGeneratedOnAdd()
					.HasColumnType("bigint")
					.HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

				b.Property<decimal>("AdministrationRights")
					.HasColumnType("decimal(20,0)");

				b.Property<long?>("GroupId")
					.HasColumnType("bigint");

				b.Property<decimal>("InstanceManagerRights")
					.HasColumnType("decimal(20,0)");

				b.Property<long?>("UserId")
					.HasColumnType("bigint");

				b.HasKey("Id");

				b.HasIndex("GroupId")
					.IsUnique()
					.HasFilter("[GroupId] IS NOT NULL");

				b.HasIndex("UserId")
					.IsUnique()
					.HasFilter("[UserId] IS NOT NULL");

				b.ToTable("PermissionSets");
			});

			modelBuilder.Entity("Tgstation.Server.Host.Models.ReattachInformation", b =>
			{
				b.Property<long>("Id")
					.ValueGeneratedOnAdd()
					.HasColumnType("bigint")
					.HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

				b.Property<string>("AccessIdentifier")
					.IsRequired()
					.HasColumnType("nvarchar(max)");

				b.Property<long>("CompileJobId")
					.HasColumnType("bigint");

				b.Property<int>("LaunchSecurityLevel")
					.HasColumnType("int");

				b.Property<int>("LaunchVisibility")
					.HasColumnType("int");

				b.Property<int>("Port")
					.HasColumnType("int");

				b.Property<int>("ProcessId")
					.HasColumnType("int");

				b.Property<int>("RebootState")
					.HasColumnType("int");

				b.HasKey("Id");

				b.HasIndex("CompileJobId");

				b.ToTable("ReattachInformations");
			});

			modelBuilder.Entity("Tgstation.Server.Host.Models.RepositorySettings", b =>
			{
				b.Property<long>("Id")
					.ValueGeneratedOnAdd()
					.HasColumnType("bigint")
					.HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

				b.Property<string>("AccessToken")
					.HasColumnType("nvarchar(max)")
					.HasMaxLength(10000);

				b.Property<string>("AccessUser")
					.HasColumnType("nvarchar(max)")
					.HasMaxLength(10000);

				b.Property<bool?>("AutoUpdatesKeepTestMerges")
					.IsRequired()
					.HasColumnType("bit");

				b.Property<bool?>("AutoUpdatesSynchronize")
					.IsRequired()
					.HasColumnType("bit");

				b.Property<string>("CommitterEmail")
					.IsRequired()
					.HasColumnType("nvarchar(max)")
					.HasMaxLength(10000);

				b.Property<string>("CommitterName")
					.IsRequired()
					.HasColumnType("nvarchar(max)")
					.HasMaxLength(10000);

				b.Property<bool?>("CreateGitHubDeployments")
					.IsRequired()
					.HasColumnType("bit");

				b.Property<long>("InstanceId")
					.HasColumnType("bigint");

				b.Property<bool?>("PostTestMergeComment")
					.IsRequired()
					.HasColumnType("bit");

				b.Property<bool?>("PushTestMergeCommits")
					.IsRequired()
					.HasColumnType("bit");

				b.Property<bool?>("ShowTestMergeCommitters")
					.IsRequired()
					.HasColumnType("bit");

				b.HasKey("Id");

				b.HasIndex("InstanceId")
					.IsUnique();

				b.ToTable("RepositorySettings");
			});

			modelBuilder.Entity("Tgstation.Server.Host.Models.RevInfoTestMerge", b =>
			{
				b.Property<long>("Id")
					.ValueGeneratedOnAdd()
					.HasColumnType("bigint")
					.HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

				b.Property<long>("RevisionInformationId")
					.HasColumnType("bigint");

				b.Property<long>("TestMergeId")
					.HasColumnType("bigint");

				b.HasKey("Id");

				b.HasIndex("RevisionInformationId");

				b.HasIndex("TestMergeId");

				b.ToTable("RevInfoTestMerges");
			});

			modelBuilder.Entity("Tgstation.Server.Host.Models.RevisionInformation", b =>
			{
				b.Property<long>("Id")
					.ValueGeneratedOnAdd()
					.HasColumnType("bigint")
					.HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

				b.Property<string>("CommitSha")
					.IsRequired()
					.HasColumnType("nvarchar(40)")
					.HasMaxLength(40);

				b.Property<long>("InstanceId")
					.HasColumnType("bigint");

				b.Property<string>("OriginCommitSha")
					.IsRequired()
					.HasColumnType("nvarchar(40)")
					.HasMaxLength(40);

				b.Property<DateTimeOffset>("Timestamp")
					.HasColumnType("datetimeoffset");

				b.HasKey("Id");

				b.HasIndex("InstanceId", "CommitSha")
					.IsUnique();

				b.ToTable("RevisionInformations");
			});

			modelBuilder.Entity("Tgstation.Server.Host.Models.TestMerge", b =>
			{
				b.Property<long>("Id")
					.ValueGeneratedOnAdd()
					.HasColumnType("bigint")
					.HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

				b.Property<string>("Author")
					.IsRequired()
					.HasColumnType("nvarchar(max)");

				b.Property<string>("BodyAtMerge")
					.IsRequired()
					.HasColumnType("nvarchar(max)");

				b.Property<string>("Comment")
					.HasColumnType("nvarchar(max)")
					.HasMaxLength(10000);

				b.Property<DateTimeOffset>("MergedAt")
					.HasColumnType("datetimeoffset");

				b.Property<long>("MergedById")
					.HasColumnType("bigint");

				b.Property<int>("Number")
					.HasColumnType("int");

				b.Property<long?>("PrimaryRevisionInformationId")
					.IsRequired()
					.HasColumnType("bigint");

				b.Property<string>("TargetCommitSha")
					.IsRequired()
					.HasColumnType("nvarchar(40)")
					.HasMaxLength(40);

				b.Property<string>("TitleAtMerge")
					.IsRequired()
					.HasColumnType("nvarchar(max)");

				b.Property<string>("Url")
					.IsRequired()
					.HasColumnType("nvarchar(max)");

				b.HasKey("Id");

				b.HasIndex("MergedById");

				b.HasIndex("PrimaryRevisionInformationId")
					.IsUnique();

				b.ToTable("TestMerges");
			});

			modelBuilder.Entity("Tgstation.Server.Host.Models.User", b =>
			{
				b.Property<long?>("Id")
					.ValueGeneratedOnAdd()
					.HasColumnType("bigint")
					.HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

				b.Property<string>("CanonicalName")
					.IsRequired()
					.HasColumnType("nvarchar(100)")
					.HasMaxLength(100);

				b.Property<DateTimeOffset?>("CreatedAt")
					.IsRequired()
					.HasColumnType("datetimeoffset");

				b.Property<long?>("CreatedById")
					.HasColumnType("bigint");

				b.Property<bool?>("Enabled")
					.IsRequired()
					.HasColumnType("bit");

				b.Property<long?>("GroupId")
					.HasColumnType("bigint");

				b.Property<DateTimeOffset?>("LastPasswordUpdate")
					.HasColumnType("datetimeoffset");

				b.Property<string>("Name")
					.IsRequired()
					.HasColumnType("nvarchar(100)")
					.HasMaxLength(100);

				b.Property<string>("PasswordHash")
					.HasColumnType("nvarchar(max)");

				b.Property<string>("SystemIdentifier")
					.HasColumnType("nvarchar(100)")
					.HasMaxLength(100);

				b.HasKey("Id");

				b.HasIndex("CanonicalName")
					.IsUnique();

				b.HasIndex("CreatedById");

				b.HasIndex("GroupId");

				b.HasIndex("SystemIdentifier")
					.IsUnique()
					.HasFilter("[SystemIdentifier] IS NOT NULL");

				b.ToTable("Users");
			});

			modelBuilder.Entity("Tgstation.Server.Host.Models.UserGroup", b =>
			{
				b.Property<long?>("Id")
					.ValueGeneratedOnAdd()
					.HasColumnType("bigint")
					.HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

				b.Property<string>("Name")
					.IsRequired()
					.HasColumnType("nvarchar(100)")
					.HasMaxLength(100);

				b.HasKey("Id");

				b.HasIndex("Name")
					.IsUnique();

				b.ToTable("Groups");
			});

			modelBuilder.Entity("Tgstation.Server.Host.Models.ChatBot", b =>
			{
				b.HasOne("Tgstation.Server.Host.Models.Instance", "Instance")
					.WithMany("ChatSettings")
					.HasForeignKey("InstanceId")
					.OnDelete(DeleteBehavior.Cascade)
					.IsRequired();
			});

			modelBuilder.Entity("Tgstation.Server.Host.Models.ChatChannel", b =>
			{
				b.HasOne("Tgstation.Server.Host.Models.ChatBot", "ChatSettings")
					.WithMany("Channels")
					.HasForeignKey("ChatSettingsId")
					.OnDelete(DeleteBehavior.Cascade)
					.IsRequired();
			});

			modelBuilder.Entity("Tgstation.Server.Host.Models.CompileJob", b =>
			{
				b.HasOne("Tgstation.Server.Host.Models.Job", "Job")
					.WithOne()
					.HasForeignKey("Tgstation.Server.Host.Models.CompileJob", "JobId")
					.OnDelete(DeleteBehavior.Cascade)
					.IsRequired();

				b.HasOne("Tgstation.Server.Host.Models.RevisionInformation", "RevisionInformation")
					.WithMany("CompileJobs")
					.HasForeignKey("RevisionInformationId")
					.OnDelete(DeleteBehavior.ClientNoAction)
					.IsRequired();
			});

			modelBuilder.Entity("Tgstation.Server.Host.Models.DreamDaemonSettings", b =>
			{
				b.HasOne("Tgstation.Server.Host.Models.Instance", "Instance")
					.WithOne("DreamDaemonSettings")
					.HasForeignKey("Tgstation.Server.Host.Models.DreamDaemonSettings", "InstanceId")
					.OnDelete(DeleteBehavior.Cascade)
					.IsRequired();
			});

			modelBuilder.Entity("Tgstation.Server.Host.Models.DreamMakerSettings", b =>
			{
				b.HasOne("Tgstation.Server.Host.Models.Instance", "Instance")
					.WithOne("DreamMakerSettings")
					.HasForeignKey("Tgstation.Server.Host.Models.DreamMakerSettings", "InstanceId")
					.OnDelete(DeleteBehavior.Cascade)
					.IsRequired();
			});

			modelBuilder.Entity("Tgstation.Server.Host.Models.InstancePermissionSet", b =>
			{
				b.HasOne("Tgstation.Server.Host.Models.Instance", "Instance")
					.WithMany("InstancePermissionSets")
					.HasForeignKey("InstanceId")
					.OnDelete(DeleteBehavior.Cascade)
					.IsRequired();

				b.HasOne("Tgstation.Server.Host.Models.PermissionSet", "PermissionSet")
					.WithMany("InstancePermissionSets")
					.HasForeignKey("PermissionSetId")
					.OnDelete(DeleteBehavior.Cascade)
					.IsRequired();
			});

			modelBuilder.Entity("Tgstation.Server.Host.Models.Job", b =>
			{
				b.HasOne("Tgstation.Server.Host.Models.User", "CancelledBy")
					.WithMany()
					.HasForeignKey("CancelledById");

				b.HasOne("Tgstation.Server.Host.Models.Instance", "Instance")
					.WithMany("Jobs")
					.HasForeignKey("InstanceId")
					.OnDelete(DeleteBehavior.Cascade)
					.IsRequired();

				b.HasOne("Tgstation.Server.Host.Models.User", "StartedBy")
					.WithMany()
					.HasForeignKey("StartedById")
					.OnDelete(DeleteBehavior.Cascade)
					.IsRequired();
			});

			modelBuilder.Entity("Tgstation.Server.Host.Models.OAuthConnection", b =>
			{
				b.HasOne("Tgstation.Server.Host.Models.User", "User")
					.WithMany("OAuthConnections")
					.HasForeignKey("UserId")
					.OnDelete(DeleteBehavior.Cascade);
			});

			modelBuilder.Entity("Tgstation.Server.Host.Models.PermissionSet", b =>
			{
				b.HasOne("Tgstation.Server.Host.Models.UserGroup", "Group")
					.WithOne("PermissionSet")
					.HasForeignKey("Tgstation.Server.Host.Models.PermissionSet", "GroupId")
					.OnDelete(DeleteBehavior.Cascade);

				b.HasOne("Tgstation.Server.Host.Models.User", "User")
					.WithOne("PermissionSet")
					.HasForeignKey("Tgstation.Server.Host.Models.PermissionSet", "UserId")
					.OnDelete(DeleteBehavior.Cascade);
			});

			modelBuilder.Entity("Tgstation.Server.Host.Models.ReattachInformation", b =>
			{
				b.HasOne("Tgstation.Server.Host.Models.CompileJob", "CompileJob")
					.WithMany()
					.HasForeignKey("CompileJobId")
					.OnDelete(DeleteBehavior.Cascade)
					.IsRequired();
			});

			modelBuilder.Entity("Tgstation.Server.Host.Models.RepositorySettings", b =>
			{
				b.HasOne("Tgstation.Server.Host.Models.Instance", "Instance")
					.WithOne("RepositorySettings")
					.HasForeignKey("Tgstation.Server.Host.Models.RepositorySettings", "InstanceId")
					.OnDelete(DeleteBehavior.Cascade)
					.IsRequired();
			});

			modelBuilder.Entity("Tgstation.Server.Host.Models.RevInfoTestMerge", b =>
			{
				b.HasOne("Tgstation.Server.Host.Models.RevisionInformation", "RevisionInformation")
					.WithMany("ActiveTestMerges")
					.HasForeignKey("RevisionInformationId")
					.OnDelete(DeleteBehavior.Cascade)
					.IsRequired();

				b.HasOne("Tgstation.Server.Host.Models.TestMerge", "TestMerge")
					.WithMany("RevisonInformations")
					.HasForeignKey("TestMergeId")
					.OnDelete(DeleteBehavior.ClientNoAction)
					.IsRequired();
			});

			modelBuilder.Entity("Tgstation.Server.Host.Models.RevisionInformation", b =>
			{
				b.HasOne("Tgstation.Server.Host.Models.Instance", "Instance")
					.WithMany("RevisionInformations")
					.HasForeignKey("InstanceId")
					.OnDelete(DeleteBehavior.Cascade)
					.IsRequired();
			});

			modelBuilder.Entity("Tgstation.Server.Host.Models.TestMerge", b =>
			{
				b.HasOne("Tgstation.Server.Host.Models.User", "MergedBy")
					.WithMany("TestMerges")
					.HasForeignKey("MergedById")
					.OnDelete(DeleteBehavior.Restrict)
					.IsRequired();

				b.HasOne("Tgstation.Server.Host.Models.RevisionInformation", "PrimaryRevisionInformation")
					.WithOne("PrimaryTestMerge")
					.HasForeignKey("Tgstation.Server.Host.Models.TestMerge", "PrimaryRevisionInformationId")
					.OnDelete(DeleteBehavior.Cascade)
					.IsRequired();
			});

			modelBuilder.Entity("Tgstation.Server.Host.Models.User", b =>
			{
				b.HasOne("Tgstation.Server.Host.Models.User", "CreatedBy")
					.WithMany("CreatedUsers")
					.HasForeignKey("CreatedById");

				b.HasOne("Tgstation.Server.Host.Models.UserGroup", "Group")
					.WithMany("Users")
					.HasForeignKey("GroupId");
			});
#pragma warning restore 612, 618
		}
	}
}

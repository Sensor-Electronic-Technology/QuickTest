using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace QuickTest.MigrateLegacy.epi;

public partial class EpiContext : DbContext
{
    public EpiContext()
    {
    }

    public EpiContext(DbContextOptions<EpiContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BurninDatum> BurninData { get; set; }

    public virtual DbSet<BurninLog> BurninLogs { get; set; }

    public virtual DbSet<EpiDataAfter> EpiDataAfters { get; set; }

    public virtual DbSet<EpiDataAfter50ma> EpiDataAfter50mas { get; set; }

    public virtual DbSet<EpiDataInitial> EpiDataInitials { get; set; }

    public virtual DbSet<EpiDataInitial50ma> EpiDataInitial50mas { get; set; }

    public virtual DbSet<EpiSpectrumAfter> EpiSpectrumAfters { get; set; }

    public virtual DbSet<EpiSpectrumInitial> EpiSpectrumInitials { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySQL("server=172.20.4.20;database=epi;uid=ewat_user;pwd=Today@epi!");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BurninDatum>(entity =>
        {
            entity.HasKey(e => e.WaferId).HasName("PRIMARY");

            entity.ToTable("burnin_data");

            entity.Property(e => e.WaferId)
                .HasMaxLength(45)
                .HasColumnName("WaferID");
            entity.Property(e => e.AFinalCurrent)
                .HasDefaultValueSql("'0'")
                .HasColumnName("a_final_current");
            entity.Property(e => e.AFinalVolt)
                .HasDefaultValueSql("'0'")
                .HasColumnName("a_final_volt");
            entity.Property(e => e.AInitCurrent)
                .HasDefaultValueSql("'0'")
                .HasColumnName("a_init_current");
            entity.Property(e => e.AInitVolt)
                .HasDefaultValueSql("'0'")
                .HasColumnName("a_init_volt");
            entity.Property(e => e.APocket)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)")
                .HasColumnName("a_pocket");
            entity.Property(e => e.ASetCurrent)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)")
                .HasColumnName("a_set_current");
            entity.Property(e => e.ASetTemp)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)")
                .HasColumnName("a_set_temp");
            entity.Property(e => e.BFinalCurrent).HasColumnName("b_final_current");
            entity.Property(e => e.BFinalVolt).HasColumnName("b_final_volt");
            entity.Property(e => e.BInitCurrent)
                .HasDefaultValueSql("'0'")
                .HasColumnName("b_init_current");
            entity.Property(e => e.BInitVolt)
                .HasDefaultValueSql("'0'")
                .HasColumnName("b_init_volt");
            entity.Property(e => e.BPocket)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)")
                .HasColumnName("b_pocket");
            entity.Property(e => e.BSetCurrent)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)")
                .HasColumnName("b_set_current");
            entity.Property(e => e.BSetTemp)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)")
                .HasColumnName("b_set_temp");
            entity.Property(e => e.BottomFinalCurrent)
                .HasDefaultValueSql("'0'")
                .HasColumnName("bottom_final_current");
            entity.Property(e => e.BottomFinalVolt)
                .HasDefaultValueSql("'0'")
                .HasColumnName("bottom_final_volt");
            entity.Property(e => e.BottomInitCurrent).HasColumnName("bottom_init_current");
            entity.Property(e => e.BottomInitVolt).HasColumnName("bottom_init_volt");
            entity.Property(e => e.BottomPocket)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)")
                .HasColumnName("bottom_pocket");
            entity.Property(e => e.BottomSetCurrent)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)")
                .HasColumnName("bottom_set_current");
            entity.Property(e => e.BottomSetTemp)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)")
                .HasColumnName("bottom_set_temp");
            entity.Property(e => e.CFinalCurrent)
                .HasDefaultValueSql("'0'")
                .HasColumnName("c_final_current");
            entity.Property(e => e.CFinalVolt)
                .HasDefaultValueSql("'0'")
                .HasColumnName("c_final_volt");
            entity.Property(e => e.CInitCurrent)
                .HasDefaultValueSql("'0'")
                .HasColumnName("c_init_current");
            entity.Property(e => e.CInitVolt)
                .HasDefaultValueSql("'0'")
                .HasColumnName("c_init_volt");
            entity.Property(e => e.CPocket)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)")
                .HasColumnName("c_pocket");
            entity.Property(e => e.CSetCurrent)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)")
                .HasColumnName("c_set_current");
            entity.Property(e => e.CSetTemp)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)")
                .HasColumnName("c_set_temp");
            entity.Property(e => e.Completed)
                .HasColumnType("bit(1)")
                .HasColumnName("completed");
            entity.Property(e => e.LFinalCurrent)
                .HasDefaultValueSql("'0'")
                .HasColumnName("l_final_current");
            entity.Property(e => e.LFinalVolt)
                .HasDefaultValueSql("'0'")
                .HasColumnName("l_final_volt");
            entity.Property(e => e.LInitCurrent)
                .HasDefaultValueSql("'0'")
                .HasColumnName("l_init_current");
            entity.Property(e => e.LInitVolt)
                .HasDefaultValueSql("'0'")
                .HasColumnName("l_init_volt");
            entity.Property(e => e.LPocket)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)")
                .HasColumnName("l_pocket");
            entity.Property(e => e.LSetCurrent)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)")
                .HasColumnName("l_set_current");
            entity.Property(e => e.LSetTemp)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)")
                .HasColumnName("l_set_temp");
            entity.Property(e => e.RFinalCurrent)
                .HasDefaultValueSql("'0'")
                .HasColumnName("r_final_current");
            entity.Property(e => e.RFinalVolt)
                .HasDefaultValueSql("'0'")
                .HasColumnName("r_final_volt");
            entity.Property(e => e.RInitCurrent)
                .HasDefaultValueSql("'0'")
                .HasColumnName("r_init_current");
            entity.Property(e => e.RInitVolt)
                .HasDefaultValueSql("'0'")
                .HasColumnName("r_init_volt");
            entity.Property(e => e.RPocket)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)")
                .HasColumnName("r_pocket");
            entity.Property(e => e.RSetCurrent)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)")
                .HasColumnName("r_set_current");
            entity.Property(e => e.RSetTemp)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)")
                .HasColumnName("r_set_temp");
            entity.Property(e => e.Running)
                .HasColumnType("bit(1)")
                .HasColumnName("running");
            entity.Property(e => e.StationId)
                .HasColumnType("int(11)")
                .HasColumnName("stationId");
            entity.Property(e => e.System).HasMaxLength(3);
            entity.Property(e => e.TFinalCurrent)
                .HasDefaultValueSql("'0'")
                .HasColumnName("t_final_current");
            entity.Property(e => e.TFinalVolt)
                .HasDefaultValueSql("'0'")
                .HasColumnName("t_final_volt");
            entity.Property(e => e.TInitCurrent)
                .HasDefaultValueSql("'0'")
                .HasColumnName("t_init_current");
            entity.Property(e => e.TInitVolt)
                .HasDefaultValueSql("'0'")
                .HasColumnName("t_init_volt");
            entity.Property(e => e.TPocket)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)")
                .HasColumnName("t_pocket");
            entity.Property(e => e.TSetCurrent)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)")
                .HasColumnName("t_set_current");
            entity.Property(e => e.TSetTemp)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)")
                .HasColumnName("t_set_temp");
        });

        modelBuilder.Entity<BurninLog>(entity =>
        {
            entity.HasKey(e => e.LogFileName).HasName("PRIMARY");

            entity.ToTable("burnin_log");

            entity.HasIndex(e => e.Wafer1, "wfk1_idx");

            entity.HasIndex(e => e.Wafer2, "wfk2_idx");

            entity.HasIndex(e => e.Wafer3, "wfk3_idx");

            entity.Property(e => e.LogFileName).HasColumnName("logFileName");
            entity.Property(e => e.Completed)
                .HasColumnType("bit(1)")
                .HasColumnName("completed");
            entity.Property(e => e.Running)
                .HasColumnType("bit(1)")
                .HasColumnName("running");
            entity.Property(e => e.SetCurrent)
                .HasColumnType("int(11)")
                .HasColumnName("set_current");
            entity.Property(e => e.SetTemp)
                .HasColumnType("int(11)")
                .HasColumnName("set_temp");
            entity.Property(e => e.StartTime)
                .HasMaxLength(6)
                .HasColumnName("start_time");
            entity.Property(e => e.Stationid)
                .HasColumnType("int(11)")
                .HasColumnName("stationid");
            entity.Property(e => e.StopTime)
                .HasMaxLength(6)
                .HasColumnName("stop_time");
            entity.Property(e => e.W1A1)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasColumnName("w1_a1");
            entity.Property(e => e.W1A2)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasColumnName("w1_a2");
            entity.Property(e => e.W2A1)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasColumnName("w2_a1");
            entity.Property(e => e.W2A2)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasColumnName("w2_a2");
            entity.Property(e => e.W3A1)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasColumnName("w3_a1");
            entity.Property(e => e.W3A2)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasColumnName("w3_a2");
            entity.Property(e => e.Wafer1).HasColumnName("wafer1");
            entity.Property(e => e.Wafer2).HasColumnName("wafer2");
            entity.Property(e => e.Wafer3).HasColumnName("wafer3");

            entity.HasOne(d => d.Wafer1Navigation).WithMany(p => p.BurninLogWafer1Navigations)
                .HasForeignKey(d => d.Wafer1)
                .HasConstraintName("wfk1");

            entity.HasOne(d => d.Wafer2Navigation).WithMany(p => p.BurninLogWafer2Navigations)
                .HasForeignKey(d => d.Wafer2)
                .HasConstraintName("wfk2");

            entity.HasOne(d => d.Wafer3Navigation).WithMany(p => p.BurninLogWafer3Navigations)
                .HasForeignKey(d => d.Wafer3)
                .HasConstraintName("wfk3");
        });

        modelBuilder.Entity<EpiDataAfter>(entity =>
        {
            entity.HasKey(e => e.WaferId).HasName("PRIMARY");

            entity.ToTable("epi_data_after");

            entity.HasIndex(e => e.System, "SystemInd");

            entity.Property(e => e.WaferId)
                .HasMaxLength(45)
                .HasColumnName("WaferID");
            entity.Property(e => e.BottomKnee).HasColumnName("Bottom_Knee");
            entity.Property(e => e.BottomPower).HasColumnName("Bottom_Power");
            entity.Property(e => e.BottomReverse)
                .HasDefaultValueSql("'0'")
                .HasColumnName("Bottom_Reverse");
            entity.Property(e => e.BottomVolt).HasColumnName("Bottom_Volt");
            entity.Property(e => e.BottomWl).HasColumnName("Bottom_WL");
            entity.Property(e => e.CenterAKnee).HasColumnName("CenterA_Knee");
            entity.Property(e => e.CenterAPower).HasColumnName("CenterA_Power");
            entity.Property(e => e.CenterAReverse)
                .HasDefaultValueSql("'0'")
                .HasColumnName("CenterA_Reverse");
            entity.Property(e => e.CenterAVolt).HasColumnName("CenterA_Volt");
            entity.Property(e => e.CenterAWl).HasColumnName("CenterA_WL");
            entity.Property(e => e.CenterBKnee).HasColumnName("CenterB_Knee");
            entity.Property(e => e.CenterBPower).HasColumnName("CenterB_Power");
            entity.Property(e => e.CenterBReverse)
                .HasDefaultValueSql("'0'")
                .HasColumnName("CenterB_Reverse");
            entity.Property(e => e.CenterBVolt).HasColumnName("CenterB_Volt");
            entity.Property(e => e.CenterBWl).HasColumnName("CenterB_WL");
            entity.Property(e => e.CenterCKnee).HasColumnName("CenterC_Knee");
            entity.Property(e => e.CenterCPower).HasColumnName("CenterC_Power");
            entity.Property(e => e.CenterCReverse)
                .HasDefaultValueSql("'0'")
                .HasColumnName("CenterC_Reverse");
            entity.Property(e => e.CenterCVolt).HasColumnName("CenterC_Volt");
            entity.Property(e => e.CenterCWl).HasColumnName("CenterC_WL");
            entity.Property(e => e.CenterDKnee).HasColumnName("CenterD_Knee");
            entity.Property(e => e.CenterDPower).HasColumnName("CenterD_Power");
            entity.Property(e => e.CenterDReverse)
                .HasDefaultValueSql("'0'")
                .HasColumnName("CenterD_Reverse");
            entity.Property(e => e.CenterDVolt).HasColumnName("CenterD_Volt");
            entity.Property(e => e.CenterDWl).HasColumnName("CenterD_WL");
            entity.Property(e => e.DateTime).HasMaxLength(6);
            entity.Property(e => e.LeftKnee).HasColumnName("Left_Knee");
            entity.Property(e => e.LeftPower).HasColumnName("Left_Power");
            entity.Property(e => e.LeftReverse)
                .HasDefaultValueSql("'0'")
                .HasColumnName("Left_Reverse");
            entity.Property(e => e.LeftVolt).HasColumnName("Left_Volt");
            entity.Property(e => e.LeftWl).HasColumnName("Left_WL");
            entity.Property(e => e.RightKnee).HasColumnName("Right_Knee");
            entity.Property(e => e.RightPower).HasColumnName("Right_Power");
            entity.Property(e => e.RightReverse)
                .HasDefaultValueSql("'0'")
                .HasColumnName("Right_Reverse");
            entity.Property(e => e.RightVolt).HasColumnName("Right_Volt");
            entity.Property(e => e.RightWl).HasColumnName("Right_WL");
            entity.Property(e => e.StationId)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)");
            entity.Property(e => e.System).HasMaxLength(3);
            entity.Property(e => e.Tested).HasColumnType("bit(1)");
            entity.Property(e => e.TopKnee).HasColumnName("Top_Knee");
            entity.Property(e => e.TopPower).HasColumnName("Top_Power");
            entity.Property(e => e.TopReverse)
                .HasDefaultValueSql("'0'")
                .HasColumnName("Top_Reverse");
            entity.Property(e => e.TopVolt).HasColumnName("Top_Volt");
            entity.Property(e => e.TopWl).HasColumnName("Top_WL");
        });

        modelBuilder.Entity<EpiDataAfter50ma>(entity =>
        {
            entity.HasKey(e => e.WaferId).HasName("PRIMARY");

            entity.ToTable("epi_data_after_50ma");

            entity.HasIndex(e => e.System, "SystemInd");

            entity.Property(e => e.WaferId)
                .HasMaxLength(45)
                .HasColumnName("WaferID");
            entity.Property(e => e.BottomKnee).HasColumnName("Bottom_Knee");
            entity.Property(e => e.BottomPower).HasColumnName("Bottom_Power");
            entity.Property(e => e.BottomReverse)
                .HasDefaultValueSql("'0'")
                .HasColumnName("Bottom_Reverse");
            entity.Property(e => e.BottomVolt).HasColumnName("Bottom_Volt");
            entity.Property(e => e.BottomWl).HasColumnName("Bottom_WL");
            entity.Property(e => e.CenterAKnee).HasColumnName("CenterA_Knee");
            entity.Property(e => e.CenterAPower).HasColumnName("CenterA_Power");
            entity.Property(e => e.CenterAReverse)
                .HasDefaultValueSql("'0'")
                .HasColumnName("CenterA_Reverse");
            entity.Property(e => e.CenterAVolt).HasColumnName("CenterA_Volt");
            entity.Property(e => e.CenterAWl).HasColumnName("CenterA_WL");
            entity.Property(e => e.CenterBKnee).HasColumnName("CenterB_Knee");
            entity.Property(e => e.CenterBPower).HasColumnName("CenterB_Power");
            entity.Property(e => e.CenterBReverse)
                .HasDefaultValueSql("'0'")
                .HasColumnName("CenterB_Reverse");
            entity.Property(e => e.CenterBVolt).HasColumnName("CenterB_Volt");
            entity.Property(e => e.CenterBWl).HasColumnName("CenterB_WL");
            entity.Property(e => e.CenterCKnee).HasColumnName("CenterC_Knee");
            entity.Property(e => e.CenterCPower).HasColumnName("CenterC_Power");
            entity.Property(e => e.CenterCReverse)
                .HasDefaultValueSql("'0'")
                .HasColumnName("CenterC_Reverse");
            entity.Property(e => e.CenterCVolt).HasColumnName("CenterC_Volt");
            entity.Property(e => e.CenterCWl).HasColumnName("CenterC_WL");
            entity.Property(e => e.CenterDKnee).HasColumnName("CenterD_Knee");
            entity.Property(e => e.CenterDPower).HasColumnName("CenterD_Power");
            entity.Property(e => e.CenterDReverse)
                .HasDefaultValueSql("'0'")
                .HasColumnName("CenterD_Reverse");
            entity.Property(e => e.CenterDVolt).HasColumnName("CenterD_Volt");
            entity.Property(e => e.CenterDWl).HasColumnName("CenterD_WL");
            entity.Property(e => e.DateTime).HasMaxLength(6);
            entity.Property(e => e.LeftKnee).HasColumnName("Left_Knee");
            entity.Property(e => e.LeftPower).HasColumnName("Left_Power");
            entity.Property(e => e.LeftReverse)
                .HasDefaultValueSql("'0'")
                .HasColumnName("Left_Reverse");
            entity.Property(e => e.LeftVolt).HasColumnName("Left_Volt");
            entity.Property(e => e.LeftWl).HasColumnName("Left_WL");
            entity.Property(e => e.RightKnee).HasColumnName("Right_Knee");
            entity.Property(e => e.RightPower).HasColumnName("Right_Power");
            entity.Property(e => e.RightReverse)
                .HasDefaultValueSql("'0'")
                .HasColumnName("Right_Reverse");
            entity.Property(e => e.RightVolt).HasColumnName("Right_Volt");
            entity.Property(e => e.RightWl).HasColumnName("Right_WL");
            entity.Property(e => e.StationId)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)");
            entity.Property(e => e.System).HasMaxLength(3);
            entity.Property(e => e.Tested).HasColumnType("bit(1)");
            entity.Property(e => e.TopKnee).HasColumnName("Top_Knee");
            entity.Property(e => e.TopPower).HasColumnName("Top_Power");
            entity.Property(e => e.TopReverse)
                .HasDefaultValueSql("'0'")
                .HasColumnName("Top_Reverse");
            entity.Property(e => e.TopVolt).HasColumnName("Top_Volt");
            entity.Property(e => e.TopWl).HasColumnName("Top_WL");
        });

        modelBuilder.Entity<EpiDataInitial>(entity =>
        {
            entity.HasKey(e => e.WaferId).HasName("PRIMARY");

            entity.ToTable("epi_data_initial");

            entity.HasIndex(e => e.System, "SystemInd");

            entity.Property(e => e.WaferId)
                .HasMaxLength(45)
                .HasColumnName("WaferID");
            entity.Property(e => e.BottomKnee).HasColumnName("Bottom_Knee");
            entity.Property(e => e.BottomPower).HasColumnName("Bottom_Power");
            entity.Property(e => e.BottomReverse)
                .HasDefaultValueSql("'0'")
                .HasColumnName("Bottom_Reverse");
            entity.Property(e => e.BottomVolt).HasColumnName("Bottom_Volt");
            entity.Property(e => e.BottomWl).HasColumnName("Bottom_WL");
            entity.Property(e => e.CenterAKnee).HasColumnName("CenterA_Knee");
            entity.Property(e => e.CenterAPower).HasColumnName("CenterA_Power");
            entity.Property(e => e.CenterAReverse)
                .HasDefaultValueSql("'0'")
                .HasColumnName("CenterA_Reverse");
            entity.Property(e => e.CenterAVolt).HasColumnName("CenterA_Volt");
            entity.Property(e => e.CenterAWl).HasColumnName("CenterA_WL");
            entity.Property(e => e.CenterBKnee).HasColumnName("CenterB_Knee");
            entity.Property(e => e.CenterBPower).HasColumnName("CenterB_Power");
            entity.Property(e => e.CenterBReverse)
                .HasDefaultValueSql("'0'")
                .HasColumnName("CenterB_Reverse");
            entity.Property(e => e.CenterBVolt).HasColumnName("CenterB_Volt");
            entity.Property(e => e.CenterBWl).HasColumnName("CenterB_WL");
            entity.Property(e => e.CenterCKnee).HasColumnName("CenterC_Knee");
            entity.Property(e => e.CenterCPower).HasColumnName("CenterC_Power");
            entity.Property(e => e.CenterCReverse)
                .HasDefaultValueSql("'0'")
                .HasColumnName("CenterC_Reverse");
            entity.Property(e => e.CenterCVolt).HasColumnName("CenterC_Volt");
            entity.Property(e => e.CenterCWl).HasColumnName("CenterC_WL");
            entity.Property(e => e.CenterDKnee).HasColumnName("CenterD_Knee");
            entity.Property(e => e.CenterDPower).HasColumnName("CenterD_Power");
            entity.Property(e => e.CenterDReverse)
                .HasDefaultValueSql("'0'")
                .HasColumnName("CenterD_Reverse");
            entity.Property(e => e.CenterDVolt).HasColumnName("CenterD_Volt");
            entity.Property(e => e.CenterDWl).HasColumnName("CenterD_WL");
            entity.Property(e => e.DateTime).HasMaxLength(6);
            entity.Property(e => e.LeftKnee).HasColumnName("Left_Knee");
            entity.Property(e => e.LeftPower).HasColumnName("Left_Power");
            entity.Property(e => e.LeftReverse)
                .HasDefaultValueSql("'0'")
                .HasColumnName("Left_Reverse");
            entity.Property(e => e.LeftVolt).HasColumnName("Left_Volt");
            entity.Property(e => e.LeftWl).HasColumnName("Left_WL");
            entity.Property(e => e.RightKnee).HasColumnName("Right_Knee");
            entity.Property(e => e.RightPower).HasColumnName("Right_Power");
            entity.Property(e => e.RightReverse)
                .HasDefaultValueSql("'0'")
                .HasColumnName("Right_Reverse");
            entity.Property(e => e.RightVolt).HasColumnName("Right_Volt");
            entity.Property(e => e.RightWl).HasColumnName("Right_WL");
            entity.Property(e => e.StationId)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)");
            entity.Property(e => e.System).HasMaxLength(3);
            entity.Property(e => e.Tested).HasColumnType("bit(1)");
            entity.Property(e => e.TopKnee).HasColumnName("Top_Knee");
            entity.Property(e => e.TopPower).HasColumnName("Top_Power");
            entity.Property(e => e.TopReverse)
                .HasDefaultValueSql("'0'")
                .HasColumnName("Top_Reverse");
            entity.Property(e => e.TopVolt).HasColumnName("Top_Volt");
            entity.Property(e => e.TopWl).HasColumnName("Top_WL");
        });

        modelBuilder.Entity<EpiDataInitial50ma>(entity =>
        {
            entity.HasKey(e => e.WaferId).HasName("PRIMARY");

            entity.ToTable("epi_data_initial_50ma");

            entity.HasIndex(e => e.System, "SystemInd");

            entity.Property(e => e.WaferId)
                .HasMaxLength(45)
                .HasColumnName("WaferID");
            entity.Property(e => e.BottomKnee).HasColumnName("Bottom_Knee");
            entity.Property(e => e.BottomPower).HasColumnName("Bottom_Power");
            entity.Property(e => e.BottomReverse)
                .HasDefaultValueSql("'0'")
                .HasColumnName("Bottom_Reverse");
            entity.Property(e => e.BottomVolt).HasColumnName("Bottom_Volt");
            entity.Property(e => e.BottomWl).HasColumnName("Bottom_WL");
            entity.Property(e => e.CenterAKnee).HasColumnName("CenterA_Knee");
            entity.Property(e => e.CenterAPower).HasColumnName("CenterA_Power");
            entity.Property(e => e.CenterAReverse)
                .HasDefaultValueSql("'0'")
                .HasColumnName("CenterA_Reverse");
            entity.Property(e => e.CenterAVolt).HasColumnName("CenterA_Volt");
            entity.Property(e => e.CenterAWl).HasColumnName("CenterA_WL");
            entity.Property(e => e.CenterBKnee).HasColumnName("CenterB_Knee");
            entity.Property(e => e.CenterBPower).HasColumnName("CenterB_Power");
            entity.Property(e => e.CenterBReverse)
                .HasDefaultValueSql("'0'")
                .HasColumnName("CenterB_Reverse");
            entity.Property(e => e.CenterBVolt).HasColumnName("CenterB_Volt");
            entity.Property(e => e.CenterBWl).HasColumnName("CenterB_WL");
            entity.Property(e => e.CenterCKnee).HasColumnName("CenterC_Knee");
            entity.Property(e => e.CenterCPower).HasColumnName("CenterC_Power");
            entity.Property(e => e.CenterCReverse)
                .HasDefaultValueSql("'0'")
                .HasColumnName("CenterC_Reverse");
            entity.Property(e => e.CenterCVolt).HasColumnName("CenterC_Volt");
            entity.Property(e => e.CenterCWl).HasColumnName("CenterC_WL");
            entity.Property(e => e.CenterDKnee).HasColumnName("CenterD_Knee");
            entity.Property(e => e.CenterDPower).HasColumnName("CenterD_Power");
            entity.Property(e => e.CenterDReverse)
                .HasDefaultValueSql("'0'")
                .HasColumnName("CenterD_Reverse");
            entity.Property(e => e.CenterDVolt).HasColumnName("CenterD_Volt");
            entity.Property(e => e.CenterDWl).HasColumnName("CenterD_WL");
            entity.Property(e => e.DateTime).HasMaxLength(6);
            entity.Property(e => e.LeftKnee).HasColumnName("Left_Knee");
            entity.Property(e => e.LeftPower).HasColumnName("Left_Power");
            entity.Property(e => e.LeftReverse)
                .HasDefaultValueSql("'0'")
                .HasColumnName("Left_Reverse");
            entity.Property(e => e.LeftVolt).HasColumnName("Left_Volt");
            entity.Property(e => e.LeftWl).HasColumnName("Left_WL");
            entity.Property(e => e.RightKnee).HasColumnName("Right_Knee");
            entity.Property(e => e.RightPower).HasColumnName("Right_Power");
            entity.Property(e => e.RightReverse)
                .HasDefaultValueSql("'0'")
                .HasColumnName("Right_Reverse");
            entity.Property(e => e.RightVolt).HasColumnName("Right_Volt");
            entity.Property(e => e.RightWl).HasColumnName("Right_WL");
            entity.Property(e => e.StationId)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)");
            entity.Property(e => e.System).HasMaxLength(3);
            entity.Property(e => e.Tested).HasColumnType("bit(1)");
            entity.Property(e => e.TopKnee).HasColumnName("Top_Knee");
            entity.Property(e => e.TopPower).HasColumnName("Top_Power");
            entity.Property(e => e.TopReverse)
                .HasDefaultValueSql("'0'")
                .HasColumnName("Top_Reverse");
            entity.Property(e => e.TopVolt).HasColumnName("Top_Volt");
            entity.Property(e => e.TopWl).HasColumnName("Top_WL");
        });

        modelBuilder.Entity<EpiSpectrumAfter>(entity =>
        {
            entity.HasKey(e => e.WaferId).HasName("PRIMARY");

            entity.ToTable("epi_spectrum_after");

            entity.Property(e => e.WaferId)
                .HasMaxLength(45)
                .HasColumnName("WaferID");
            entity.Property(e => e.BottomSpect)
                .HasColumnType("json")
                .HasColumnName("Bottom_Spect");
            entity.Property(e => e.BottomSpect50mA)
                .HasColumnType("json")
                .HasColumnName("Bottom_Spect_50mA");
            entity.Property(e => e.BottomWl)
                .HasColumnType("json")
                .HasColumnName("Bottom_WL");
            entity.Property(e => e.BottomWl50mA)
                .HasColumnType("json")
                .HasColumnName("Bottom_WL_50mA");
            entity.Property(e => e.CenterASpect)
                .HasColumnType("json")
                .HasColumnName("CenterA_Spect");
            entity.Property(e => e.CenterASpect50mA)
                .HasColumnType("json")
                .HasColumnName("CenterA_Spect_50mA");
            entity.Property(e => e.CenterAWl)
                .HasColumnType("json")
                .HasColumnName("CenterA_WL");
            entity.Property(e => e.CenterAWl50mA)
                .HasColumnType("json")
                .HasColumnName("CenterA_WL_50mA");
            entity.Property(e => e.CenterBSpect)
                .HasColumnType("json")
                .HasColumnName("CenterB_Spect");
            entity.Property(e => e.CenterBSpect50mA)
                .HasColumnType("json")
                .HasColumnName("CenterB_Spect_50mA");
            entity.Property(e => e.CenterBWl)
                .HasColumnType("json")
                .HasColumnName("CenterB_WL");
            entity.Property(e => e.CenterBWl50mA)
                .HasColumnType("json")
                .HasColumnName("CenterB_WL_50mA");
            entity.Property(e => e.CenterCSpect)
                .HasColumnType("json")
                .HasColumnName("CenterC_Spect");
            entity.Property(e => e.CenterCSpect50mA)
                .HasColumnType("json")
                .HasColumnName("CenterC_Spect_50mA");
            entity.Property(e => e.CenterCWl)
                .HasColumnType("json")
                .HasColumnName("CenterC_WL");
            entity.Property(e => e.CenterCWl50mA)
                .HasColumnType("json")
                .HasColumnName("CenterC_WL_50mA");
            entity.Property(e => e.CenterDSpect)
                .HasColumnType("json")
                .HasColumnName("CenterD_Spect");
            entity.Property(e => e.CenterDSpect50mA)
                .HasColumnType("json")
                .HasColumnName("CenterD_Spect_50mA");
            entity.Property(e => e.CenterDWl)
                .HasColumnType("json")
                .HasColumnName("CenterD_WL");
            entity.Property(e => e.CenterDWl50mA)
                .HasColumnType("json")
                .HasColumnName("CenterD_WL_50mA");
            entity.Property(e => e.DateTime).HasMaxLength(6);
            entity.Property(e => e.LeftSpect)
                .HasColumnType("json")
                .HasColumnName("Left_Spect");
            entity.Property(e => e.LeftSpect50mA)
                .HasColumnType("json")
                .HasColumnName("Left_Spect_50mA");
            entity.Property(e => e.LeftWl)
                .HasColumnType("json")
                .HasColumnName("Left_WL");
            entity.Property(e => e.LeftWl50mA)
                .HasColumnType("json")
                .HasColumnName("Left_WL_50mA");
            entity.Property(e => e.RightSpect)
                .HasColumnType("json")
                .HasColumnName("Right_Spect");
            entity.Property(e => e.RightSpect50mA)
                .HasColumnType("json")
                .HasColumnName("Right_Spect_50mA");
            entity.Property(e => e.RightWl)
                .HasColumnType("json")
                .HasColumnName("Right_WL");
            entity.Property(e => e.RightWl50mA)
                .HasColumnType("json")
                .HasColumnName("Right_WL_50mA");
            entity.Property(e => e.System).HasMaxLength(3);
            entity.Property(e => e.Tested).HasColumnType("bit(1)");
            entity.Property(e => e.TopSpect)
                .HasColumnType("json")
                .HasColumnName("Top_Spect");
            entity.Property(e => e.TopSpect50mA)
                .HasColumnType("json")
                .HasColumnName("Top_Spect_50mA");
            entity.Property(e => e.TopWl)
                .HasColumnType("json")
                .HasColumnName("Top_WL");
            entity.Property(e => e.TopWl50mA)
                .HasColumnType("json")
                .HasColumnName("Top_WL_50mA");
        });

        modelBuilder.Entity<EpiSpectrumInitial>(entity =>
        {
            entity.HasKey(e => e.WaferId).HasName("PRIMARY");

            entity.ToTable("epi_spectrum_initial");

            entity.Property(e => e.WaferId)
                .HasMaxLength(45)
                .HasColumnName("WaferID");
            entity.Property(e => e.BottomSpect)
                .HasColumnType("json")
                .HasColumnName("Bottom_Spect");
            entity.Property(e => e.BottomSpect50mA)
                .HasColumnType("json")
                .HasColumnName("Bottom_Spect_50mA");
            entity.Property(e => e.BottomWl)
                .HasColumnType("json")
                .HasColumnName("Bottom_WL");
            entity.Property(e => e.BottomWl50mA)
                .HasColumnType("json")
                .HasColumnName("Bottom_WL_50mA");
            entity.Property(e => e.CenterASpect)
                .HasColumnType("json")
                .HasColumnName("CenterA_Spect");
            entity.Property(e => e.CenterASpect50mA)
                .HasColumnType("json")
                .HasColumnName("CenterA_Spect_50mA");
            entity.Property(e => e.CenterAWl)
                .HasColumnType("json")
                .HasColumnName("CenterA_WL");
            entity.Property(e => e.CenterAWl50mA)
                .HasColumnType("json")
                .HasColumnName("CenterA_WL_50mA");
            entity.Property(e => e.CenterBSpect)
                .HasColumnType("json")
                .HasColumnName("CenterB_Spect");
            entity.Property(e => e.CenterBSpect50mA)
                .HasColumnType("json")
                .HasColumnName("CenterB_Spect_50mA");
            entity.Property(e => e.CenterBWl)
                .HasColumnType("json")
                .HasColumnName("CenterB_WL");
            entity.Property(e => e.CenterBWl50mA)
                .HasColumnType("json")
                .HasColumnName("CenterB_WL_50mA");
            entity.Property(e => e.CenterCSpect)
                .HasColumnType("json")
                .HasColumnName("CenterC_Spect");
            entity.Property(e => e.CenterCSpect50mA)
                .HasColumnType("json")
                .HasColumnName("CenterC_Spect_50mA");
            entity.Property(e => e.CenterCWl)
                .HasColumnType("json")
                .HasColumnName("CenterC_WL");
            entity.Property(e => e.CenterCWl50mA)
                .HasColumnType("json")
                .HasColumnName("CenterC_WL_50mA");
            entity.Property(e => e.CenterDSpect)
                .HasColumnType("json")
                .HasColumnName("CenterD_Spect");
            entity.Property(e => e.CenterDSpect50mA)
                .HasColumnType("json")
                .HasColumnName("CenterD_Spect_50mA");
            entity.Property(e => e.CenterDWl)
                .HasColumnType("json")
                .HasColumnName("CenterD_WL");
            entity.Property(e => e.CenterDWl50mA)
                .HasColumnType("json")
                .HasColumnName("CenterD_WL_50mA");
            entity.Property(e => e.DateTime).HasMaxLength(6);
            entity.Property(e => e.LeftSpect)
                .HasColumnType("json")
                .HasColumnName("Left_Spect");
            entity.Property(e => e.LeftSpect50mA)
                .HasColumnType("json")
                .HasColumnName("Left_Spect_50mA");
            entity.Property(e => e.LeftWl)
                .HasColumnType("json")
                .HasColumnName("Left_WL");
            entity.Property(e => e.LeftWl50mA)
                .HasColumnType("json")
                .HasColumnName("Left_WL_50mA");
            entity.Property(e => e.RightSpect)
                .HasColumnType("json")
                .HasColumnName("Right_Spect");
            entity.Property(e => e.RightSpect50mA)
                .HasColumnType("json")
                .HasColumnName("Right_Spect_50mA");
            entity.Property(e => e.RightWl)
                .HasColumnType("json")
                .HasColumnName("Right_WL");
            entity.Property(e => e.RightWl50mA)
                .HasColumnType("json")
                .HasColumnName("Right_WL_50mA");
            entity.Property(e => e.StationId)
                .HasDefaultValueSql("'0'")
                .HasColumnType("int(11)");
            entity.Property(e => e.System).HasMaxLength(3);
            entity.Property(e => e.Tested).HasColumnType("bit(1)");
            entity.Property(e => e.TopSpect)
                .HasColumnType("json")
                .HasColumnName("Top_Spect");
            entity.Property(e => e.TopSpect50mA)
                .HasColumnType("json")
                .HasColumnName("Top_Spect_50mA");
            entity.Property(e => e.TopWl)
                .HasColumnType("json")
                .HasColumnName("Top_WL");
            entity.Property(e => e.TopWl50mA)
                .HasColumnType("json")
                .HasColumnName("Top_WL_50mA");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [departments] (
    [DepartmentId] int NOT NULL IDENTITY,
    [DepartmentName] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_departments] PRIMARY KEY ([DepartmentId])
);

CREATE TABLE [users] (
    [UserId] int NOT NULL IDENTITY,
    [Role] nvarchar(max) NOT NULL,
    [FullName] nvarchar(max) NOT NULL,
    [Gender] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [PhoneNumber] nvarchar(max) NOT NULL,
    [Password] nvarchar(max) NOT NULL,
    [IsDeleted] bit NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpadatedDate] datetime2 NULL,
    [UpdatedBy] int NULL,
    CONSTRAINT [PK_users] PRIMARY KEY ([UserId])
);

CREATE TABLE [admins] (
    [AdminId] int NOT NULL IDENTITY,
    [Role] nvarchar(max) NOT NULL,
    [FullName] nvarchar(max) NOT NULL,
    [Gender] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [PhoneNumber] nvarchar(max) NOT NULL,
    [Password] nvarchar(max) NOT NULL,
    [UserId] int NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpadatedDate] datetime2 NULL,
    [UpdatedBy] int NULL,
    CONSTRAINT [PK_admins] PRIMARY KEY ([AdminId]),
    CONSTRAINT [FK_admins_users_UserId] FOREIGN KEY ([UserId]) REFERENCES [users] ([UserId]) ON DELETE CASCADE
);

CREATE TABLE [doctors] (
    [DoctorId] int NOT NULL IDENTITY,
    [Role] nvarchar(max) NOT NULL,
    [FullName] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [PhoneNumber] nvarchar(max) NOT NULL,
    [Gender] nvarchar(max) NOT NULL,
    [DepartmentId] int NOT NULL,
    [Password] nvarchar(max) NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpadatedDate] datetime2 NULL,
    [UpdatedBy] int NULL,
    [UserId] int NOT NULL,
    CONSTRAINT [PK_doctors] PRIMARY KEY ([DoctorId]),
    CONSTRAINT [FK_doctors_departments_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [departments] ([DepartmentId]) ON DELETE CASCADE,
    CONSTRAINT [FK_doctors_users_UserId] FOREIGN KEY ([UserId]) REFERENCES [users] ([UserId]) ON DELETE CASCADE
);

CREATE TABLE [patients] (
    [PatientId] int NOT NULL IDENTITY,
    [Role] nvarchar(max) NOT NULL,
    [FullName] nvarchar(max) NOT NULL,
    [Gender] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [PhoneNumber] nvarchar(max) NOT NULL,
    [Password] nvarchar(max) NOT NULL,
    [IsDeleted] bit NOT NULL,
    [UserId] int NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpadatedDate] datetime2 NULL,
    [UpdatedBy] int NULL,
    CONSTRAINT [PK_patients] PRIMARY KEY ([PatientId]),
    CONSTRAINT [FK_patients_users_UserId] FOREIGN KEY ([UserId]) REFERENCES [users] ([UserId]) ON DELETE CASCADE
);

CREATE TABLE [receptionists] (
    [ReceptionistId] int NOT NULL IDENTITY,
    [Role] nvarchar(max) NOT NULL,
    [FullName] nvarchar(max) NOT NULL,
    [Gender] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [PhoneNumber] nvarchar(max) NOT NULL,
    [Password] nvarchar(max) NOT NULL,
    [UserId] int NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpadatedDate] datetime2 NULL,
    [UpdatedBy] int NULL,
    CONSTRAINT [PK_receptionists] PRIMARY KEY ([ReceptionistId]),
    CONSTRAINT [FK_receptionists_users_UserId] FOREIGN KEY ([UserId]) REFERENCES [users] ([UserId]) ON DELETE CASCADE
);

CREATE TABLE [doctorOnLeaves] (
    [LeaveId] int NOT NULL IDENTITY,
    [DoctorId] int NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpadatedDate] datetime2 NULL,
    [StartDate] datetime2 NOT NULL,
    [EndDate] datetime2 NOT NULL,
    [Reason] nvarchar(max) NOT NULL,
    [OnLeave] bit NOT NULL,
    CONSTRAINT [PK_doctorOnLeaves] PRIMARY KEY ([LeaveId]),
    CONSTRAINT [FK_doctorOnLeaves_doctors_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [doctors] ([DoctorId]) ON DELETE CASCADE
);

CREATE TABLE [appointments] (
    [AppointmentId] int NOT NULL IDENTITY,
    [PatientId] int NULL,
    [DoctorId] int NOT NULL,
    [AppointmentDate] datetime2 NOT NULL,
    [RescheduledAt] datetime2 NULL,
    [SlotTime] nvarchar(max) NOT NULL,
    [AppointmentBooked] bit NOT NULL,
    [AppointmentRescheduled] bit NOT NULL,
    [AppointmentCancled] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [UpdatedBy] int NULL,
    CONSTRAINT [PK_appointments] PRIMARY KEY ([AppointmentId]),
    CONSTRAINT [FK_appointments_doctors_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [doctors] ([DoctorId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_appointments_patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [patients] ([PatientId]) ON DELETE NO ACTION
);

CREATE TABLE [prescriptions] (
    [PrescriptionId] int NOT NULL IDENTITY,
    [PatientId] int NOT NULL,
    [DoctorId] int NOT NULL,
    [Decease] nvarchar(max) NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_prescriptions] PRIMARY KEY ([PrescriptionId]),
    CONSTRAINT [FK_prescriptions_doctors_DoctorId] FOREIGN KEY ([DoctorId]) REFERENCES [doctors] ([DoctorId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_prescriptions_patients_PatientId] FOREIGN KEY ([PatientId]) REFERENCES [patients] ([PatientId]) ON DELETE NO ACTION
);

CREATE TABLE [prescribedMedicines] (
    [Id] int NOT NULL IDENTITY,
    [StartDate] datetime2 NOT NULL,
    [EndDate] datetime2 NOT NULL,
    [MedicineName] nvarchar(max) NOT NULL,
    [TimesPerDay] int NOT NULL,
    [PrescriptionModelPrescriptionId] int NULL,
    CONSTRAINT [PK_prescribedMedicines] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_prescribedMedicines_prescriptions_PrescriptionModelPrescriptionId] FOREIGN KEY ([PrescriptionModelPrescriptionId]) REFERENCES [prescriptions] ([PrescriptionId])
);

CREATE INDEX [IX_admins_UserId] ON [admins] ([UserId]);

CREATE INDEX [IX_appointments_DoctorId] ON [appointments] ([DoctorId]);

CREATE INDEX [IX_appointments_PatientId] ON [appointments] ([PatientId]);

CREATE INDEX [IX_doctorOnLeaves_DoctorId] ON [doctorOnLeaves] ([DoctorId]);

CREATE INDEX [IX_doctors_DepartmentId] ON [doctors] ([DepartmentId]);

CREATE INDEX [IX_doctors_UserId] ON [doctors] ([UserId]);

CREATE INDEX [IX_patients_UserId] ON [patients] ([UserId]);

CREATE INDEX [IX_prescribedMedicines_PrescriptionModelPrescriptionId] ON [prescribedMedicines] ([PrescriptionModelPrescriptionId]);

CREATE INDEX [IX_prescriptions_DoctorId] ON [prescriptions] ([DoctorId]);

CREATE INDEX [IX_prescriptions_PatientId] ON [prescriptions] ([PatientId]);

CREATE INDEX [IX_receptionists_UserId] ON [receptionists] ([UserId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250715105646_init', N'9.0.6');

COMMIT;
GO


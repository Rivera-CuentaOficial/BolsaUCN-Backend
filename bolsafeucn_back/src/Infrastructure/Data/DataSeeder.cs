using Bogus;
using bolsafeucn_back.src.Domain.Models;
using bolsafeucn_back.src.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace bolsafeucn_back.src.Application.Infrastructure.Data
{
    public class DataSeeder
    {
        public static async Task Initialize(
            IConfiguration configuration,
            IServiceProvider serviceProvider
        )
        {
            var context = serviceProvider.GetRequiredService<AppDbContext>();
            try
            {
                var userManager = serviceProvider.GetRequiredService<UserManager<GeneralUser>>();
                var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();

                Log.Information("DataSeeder: Iniciando la migración de la base de datos...");
                await context.Database.MigrateAsync();
                Log.Information("DataSeeder: Migración de la base de datos completada.");

                if (!await context.Roles.AnyAsync())
                {
                    Log.Information("DataSeeder: No se encontraron roles, creando roles...");
                    var roles = new List<Role>
                    {
                        new Role { Name = "Admin", NormalizedName = "ADMIN" },
                        new Role { Name = "Applicant", NormalizedName = "APPLICANT" },
                        new Role { Name = "Offerent", NormalizedName = "OFFERENT" },
                        new Role { Name = "SuperAdmin", NormalizedName = "SUPERADMIN" },
                    };
                    foreach (var role in roles)
                    {
                        await roleManager.CreateAsync(role);
                    }
                    Log.Information("DataSeeder: Roles creados exitosamente.");
                }

                if (!await context.Users.AnyAsync())
                {
                    Log.Information(
                        "DataSeeder: No se encontraron usuarios, creando usuarios de prueba..."
                    );
                    await SeedUsers(userManager, context);
                    Log.Information("DataSeeder: Usuarios de prueba creados exitosamente.");
                }

                if (!await context.Offers.AnyAsync())
                {
                    Log.Information(
                        "DataSeeder: No se encontraron ofertas, creando ofertas de prueba..."
                    );
                    await SeedOffers(context);
                    Log.Information("DataSeeder: Ofertas de prueba creadas exitosamente.");
                }
                if (!await context.BuySells.AnyAsync())
                {
                    Log.Information(
                        "DataSeeder: No hay avisos de compra/venta, creando datos de prueba..."
                    );
                    await SeedBuySells(context);
                    Log.Information("DataSeeder: Compra/venta de prueba creados.");
                }
                if (!await context.JobApplications.AnyAsync())
                {
                    Log.Information(
                        "DataSeeder: No se encontraron postulaciones, creando postulaciones de prueba..."
                    );
                    await SeedJobApplications(context);
                    Log.Information("DataSeeder: Postulaciones de prueba creadas exitosamente.");
                }
                if (!await context.Reviews.AnyAsync())
                {
                    Log.Information("DataSeeder: No se encontraron reviews, creando reviews de prueba...");
                    await SeedReviews(context);
                    Log.Information("DataSeeder: Reviews de prueba creadas exitosamente.");
                }
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "DataSeeder: Error al inicializar la base de datos.");
                throw;
            }
        }

        private static async Task SeedUsers(
            UserManager<GeneralUser> userManager,
            AppDbContext context
        )
        {
            var faker = new Faker("es");

            // ========================================
            // USUARIOS DE PRUEBA CON CREDENCIALES FÁCILES
            // ========================================
            Log.Information("DataSeeder: Creando usuarios de prueba con credenciales fáciles...");

            // 1. ESTUDIANTE DE PRUEBA
            var testStudentUser = new GeneralUser
            {
                UserName = "estudiante",
                Email = "estudiante@alumnos.ucn.cl",
                PhoneNumber = "+56912345678",
                UserType = UserType.Estudiante,
                Rut = "12345678-9",
                EmailConfirmed = true,
                Banned = false,
            };
            var studentResult = await userManager.CreateAsync(testStudentUser, "Test123!");
            if (studentResult.Succeeded)
            {
                await userManager.AddToRoleAsync(testStudentUser, "Applicant");
                var testStudent = new Student
                {
                    GeneralUserId = testStudentUser.Id,
                    Name = "Juan",
                    LastName = "Pérez Estudiante",
                    Disability = Disability.Ninguna,
                    GeneralUser = testStudentUser,
                    CurriculumVitae = "https://ejemplo.com/cv/juan_perez.pdf", // CV de prueba
                    MotivationLetter = "Soy un estudiante motivado y con ganas de aprender", // Carta opcional
                };
                context.Students.Add(testStudent);
                Log.Information(
                    "✅ Usuario estudiante creado: estudiante@alumnos.ucn.cl / Test123!"
                );
            }

            // 2. EMPRESA DE PRUEBA
            var testCompanyUser = new GeneralUser
            {
                UserName = "empresa",
                Email = "empresa@techcorp.cl",
                PhoneNumber = "+56987654321",
                UserType = UserType.Empresa,
                Rut = "76543210-K",
                EmailConfirmed = true,
                Banned = false,
            };
            var companyResult = await userManager.CreateAsync(testCompanyUser, "Test123!");
            if (companyResult.Succeeded)
            {
                await userManager.AddToRoleAsync(testCompanyUser, "Offerent");
                var testCompany = new Company
                {
                    GeneralUserId = testCompanyUser.Id,
                    CompanyName = "Tech Corp SpA",
                    LegalName = "Tecnología Corporativa SpA",
                    GeneralUser = testCompanyUser,
                };
                context.Companies.Add(testCompany);
                Log.Information("✅ Usuario empresa creado: empresa@techcorp.cl / Test123!");
            }

            // 3. PARTICULAR DE PRUEBA
            var testIndividualUser = new GeneralUser
            {
                UserName = "particular",
                Email = "particular@ucn.cl",
                PhoneNumber = "+56955555555",
                UserType = UserType.Particular,
                Rut = "11222333-4",
                EmailConfirmed = true,
                Banned = false,
            };
            var individualResult = await userManager.CreateAsync(testIndividualUser, "Test123!");
            if (individualResult.Succeeded)
            {
                await userManager.AddToRoleAsync(testIndividualUser, "Offerent");
                var testIndividual = new Individual
                {
                    GeneralUserId = testIndividualUser.Id,
                    Name = "María",
                    LastName = "González Particular",
                    GeneralUser = testIndividualUser,
                };
                context.Individuals.Add(testIndividual);
                Log.Information("✅ Usuario particular creado: particular@ucn.cl / Test123!");
            }

            // 4. ADMIN DE PRUEBA
            var testAdminUser = new GeneralUser
            {
                UserName = "admin",
                Email = "admin@ucn.cl",
                PhoneNumber = "+56911111111",
                UserType = UserType.Administrador,
                Rut = "99888777-6",
                EmailConfirmed = true,
                Banned = false,
            };
            var adminResult = await userManager.CreateAsync(testAdminUser, "Test123!");
            if (adminResult.Succeeded)
            {
                await userManager.AddToRoleAsync(testAdminUser, "Admin");
                var testAdmin = new Admin
                {
                    GeneralUserId = testAdminUser.Id,
                    Name = "Carlos",
                    LastName = "Admin Sistema",
                    SuperAdmin = false,
                    GeneralUser = testAdminUser,
                };
                context.Admins.Add(testAdmin);
                Log.Information("✅ Usuario admin creado: admin@ucn.cl / Test123!");
            }

            Log.Information("DataSeeder: Usuarios de prueba creados exitosamente.");
            Log.Information("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Log.Information("📋 CREDENCIALES DE PRUEBA:");
            Log.Information("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Log.Information("👨‍🎓 ESTUDIANTE:");
            Log.Information("   Email: estudiante@alumnos.ucn.cl");
            Log.Information("   Pass:  Test123!");
            Log.Information("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Log.Information("🏢 EMPRESA:");
            Log.Information("   Email: empresa@techcorp.cl");
            Log.Information("   Pass:  Test123!");
            Log.Information("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Log.Information("👤 PARTICULAR:");
            Log.Information("   Email: particular@ucn.cl");
            Log.Information("   Pass:  Test123!");
            Log.Information("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Log.Information("👑 ADMIN:");
            Log.Information("   Email: admin@ucn.cl");
            Log.Information("   Pass:  Test123!");
            Log.Information("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

            // ========================================
            // USUARIOS ALEATORIOS ADICIONALES (Faker)
            // ========================================
            Log.Information("DataSeeder: Creando usuarios aleatorios adicionales...");

            // Seed Random Students
            for (int i = 0; i < 3; i++)
            {
                var studentUser = new GeneralUser
                {
                    UserName = faker.Internet.UserName(),
                    Email = faker.Internet.Email(),
                    PhoneNumber = faker.Phone.PhoneNumber("+569########"),
                    UserType = UserType.Estudiante,
                    Rut = faker.Random.Replace("##.###.###-K"),
                    EmailConfirmed = true,
                    Banned = faker.Random.Bool(0.3f),
                };
                var result = await userManager.CreateAsync(studentUser, "Password123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(studentUser, "Applicant");
                    var student = new Student
                    {
                        GeneralUserId = studentUser.Id,
                        Name = faker.Name.FirstName(),
                        LastName = faker.Name.LastName(),
                        Disability = faker.PickRandom<Disability>(),
                        GeneralUser = studentUser,
                    };
                    context.Students.Add(student);
                }
            }

            // Seed Companies
            for (int i = 0; i < 2; i++)
            {
                var companyUser = new GeneralUser
                {
                    UserName = faker.Internet.UserName(),
                    Email = faker.Internet.Email(),
                    UserType = UserType.Empresa,
                    Rut = faker.Random.Replace("##.###.###-K"),
                    EmailConfirmed = true,
                    Banned = faker.Random.Bool(0.3f),
                };
                var result = await userManager.CreateAsync(companyUser, "Password123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(companyUser, "Offerent");
                    var company = new Company
                    {
                        GeneralUserId = companyUser.Id,
                        CompanyName = faker.Company.CompanyName(),
                        LegalName = faker.Company.CompanyName() + " S.A.",
                        GeneralUser = companyUser,
                    };
                    context.Companies.Add(company);
                }
            }

            // Seed Individual
            var randomIndividualUser = new GeneralUser
            {
                UserName = faker.Internet.UserName(),
                Email = faker.Internet.Email(),
                UserType = UserType.Particular,
                Rut = faker.Random.Replace("##.###.###-K"),
                EmailConfirmed = true,
                Banned = faker.Random.Bool(0.9f),
            };
            var randomIndividualResult = await userManager.CreateAsync(
                randomIndividualUser,
                "Password123!"
            );
            if (randomIndividualResult.Succeeded)
            {
                await userManager.AddToRoleAsync(randomIndividualUser, "Offerent");
                var randomIndividual = new Individual
                {
                    GeneralUserId = randomIndividualUser.Id,
                    Name = faker.Name.FirstName(),
                    LastName = faker.Name.LastName(),
                    GeneralUser = randomIndividualUser,
                };
                context.Individuals.Add(randomIndividual);
            }

            await context.SaveChangesAsync();
            Log.Information("DataSeeder: Todos los usuarios creados exitosamente.");
        }

        private static async Task SeedOffers(AppDbContext context)
        {
            var offerents = await context
                .Users.Where(u =>
                    u.UserType == UserType.Empresa || u.UserType == UserType.Particular
                )
                .ToListAsync();

            if (offerents.Count == 0)
                return;

            var now = DateTime.UtcNow;

            // Muestras curadas (todo en castellano y con campos útiles)
            var samples = new[]
            {
                new
                {
                    Title = "Apoyo en Feria UCN",
                    Desc = "Logística de stands, orientación a asistentes y apoyo en acreditación.",
                    Rem = 55000,
                    Type = OfferTypes.Trabajo,
                    Loc = "Campus Antofagasta",
                    Req = "Responsable, trato cordial, disponibilidad el sábado.",
                    Contact = "feucn@ucn.cl",
                    IsCv = false,
                    Deadline = now.AddDays(7),
                    End = now.AddDays(10),
                },
                new
                {
                    Title = "Diseño de flyer (freelance)",
                    Desc = "Diseño de pieza gráfica en formato A4 y versión para RRSS.",
                    Rem = 40000,
                    Type = OfferTypes.Trabajo,
                    Loc = "Remoto",
                    Req = "Portafolio o muestras previas; entrega en 48h.",
                    Contact = "deportes@ucn.cl",
                    IsCv = false,
                    Deadline = now.AddDays(5),
                    End = now.AddDays(7),
                },
                new
                {
                    Title = "Tutorías de Cálculo I",
                    Desc = "Tutorías grupales (máx. 8) dos veces por semana durante 1 mes.",
                    Rem = 0,
                    Type = OfferTypes.Voluntariado,
                    Loc = "Campus Coquimbo",
                    Req = "Aprobado Cálculo I/II, ganas de explicar.",
                    Contact = "centro.estudiantes@ucn.cl",
                    IsCv = true,
                    Deadline = now.AddDays(9),
                    End = now.AddDays(30),
                },
                new
                {
                    Title = "Community Manager para evento",
                    Desc = "Cobertura en vivo y publicaciones previas del evento (1 semana).",
                    Rem = 80000,
                    Type = OfferTypes.Trabajo,
                    Loc = "Remoto / Híbrido",
                    Req = "Manejo de IG y TikTok; redacción básica.",
                    Contact = "comunicaciones@ucn.cl",
                    IsCv = true,
                    Deadline = now.AddDays(6),
                    End = now.AddDays(12),
                },
                new
                {
                    Title = "Asistente de Laboratorio (química)",
                    Desc = "Apoyo en preparación de materiales y registro de datos.",
                    Rem = 120000,
                    Type = OfferTypes.Trabajo,
                    Loc = "Campus Antofagasta",
                    Req = "Ramos básicos aprobados; EPP al día.",
                    Contact = "lab.quimica@ucn.cl",
                    IsCv = true,
                    Deadline = now.AddDays(10),
                    End = now.AddDays(20),
                },
                new
                {
                    Title = "Mentorías a mechones (Programa Bienestar)",
                    Desc = "Acompañamiento y resolución de dudas generales 1 vez por semana.",
                    Rem = 0,
                    Type = OfferTypes.Voluntariado,
                    Loc = "Campus Coquimbo",
                    Req = "Segundo año o superior; empatía y responsabilidad.",
                    Contact = "bienestar@ucn.cl",
                    IsCv = false,
                    Deadline = now.AddDays(8),
                    End = now.AddDays(40),
                },
            };

            context.Offers.RemoveRange(context.Offers);
            await context.SaveChangesAsync();

            int i = 0;
            foreach (var s in samples)
            {
                var owner = offerents[i++ % offerents.Count];

                var offer = new Offer
                {
                    UserId = owner.Id,
                    User = owner,

                    Title = s.Title,
                    Description = s.Desc,
                    PublicationDate = now.AddDays(-i % 3), // algunas “recientes”
                    Type = Types.Offer,
                    IsActive = true,
                    statusValidation = StatusValidation.Published,

                    EndDate = s.End,
                    DeadlineDate = s.Deadline,
                    Remuneration = s.Rem,
                    OfferType = s.Type,
                    Location = s.Loc,
                    Requirements = s.Req,
                    ContactInfo = s.Contact,
                    IsCvRequired = s.IsCv,
                };

                context.Offers.Add(offer);
            }

            // <<< INICIO: OFERTA "INPROCESS" SOLICITADA >>>
            var firstOfferent = offerents.First();
            var inProcessOffer = new Offer
            {
                UserId = firstOfferent.Id,
                User = firstOfferent,

                Title = "Práctica Desarrollo .NET (En Revisión)",
                Description =
                    "Se busca estudiante para práctica de 3 meses en desarrollo backend con .NET y Azure. El postulante debe estar en último año. Esta oferta está pendiente de aprobación por la DGE.",
                PublicationDate = now.AddDays(-1),
                Type = Types.Offer,
                IsActive = true,
                statusValidation = StatusValidation.InProcess, // <- Estado solicitado

                EndDate = now.AddMonths(3),
                DeadlineDate = now.AddDays(14),
                Remuneration = 400000, // Remuneración de práctica
                OfferType = OfferTypes.Trabajo, // Asumiendo que Práctica es un tipo de Trabajo
                Location = "Remoto (Chile)",
                Requirements =
                    "Cursando último año. Conocimiento en C# y SQL Server. Deseable Azure.",
                ContactInfo = "rrhh.pending@techcorp.cl",
                IsCvRequired = true,
            };
            context.Offers.Add(inProcessOffer);
            // <<< FIN: OFERTA "INPROCESS" SOLICITADA >>>

            await context.SaveChangesAsync();
            Log.Information(
                "DataSeeder: Ofertas de ejemplo cargadas ({Count})",
                samples.Length + 1
            ); // +1 por la nueva
        }

        private static async Task SeedBuySells(AppDbContext context)
        {
            var now = DateTime.UtcNow;

            // Buscamos oferentes (empresa o particular) para asociar publicaciones
            var sellers = await context
                .Users.Where(u =>
                    u.UserType == UserType.Empresa || u.UserType == UserType.Particular
                )
                .ToListAsync();
            if (sellers.Count == 0)
                return;

            // Muestras curadas (campos útiles y en castellano)
            var items = new[]
            {
                new
                {
                    Title = "Venta libro Cálculo I (Stewart 7ma)",
                    Desc = "En buen estado, pocas marcas.",
                    Price = 12000m,
                    Category = "Libros",
                    Loc = "Antofagasta",
                    Contact = "ignacio@ucn.cl",
                },
                new
                {
                    Title = "Teclado mecánico Redragon K552",
                    Desc = "Switch blue, 1 año de uso.",
                    Price = 18000m,
                    Category = "Tecnología",
                    Loc = "Coquimbo",
                    Contact = "+56987654321",
                },
                new
                {
                    Title = "Bata laboratorio talla M",
                    Desc = "Lavada y desinfectada, casi nueva.",
                    Price = 8000m,
                    Category = "Laboratorio",
                    Loc = "Antofagasta",
                    Contact = "c.labs@ucn.cl",
                },
                new
                {
                    Title = "Calculadora científica Casio fx-82",
                    Desc = "Funciona perfecto, con pilas nuevas.",
                    Price = 9000m,
                    Category = "Accesorios",
                    Loc = "Remoto",
                    Contact = "ventas@ucn.cl",
                },
                new
                {
                    Title = "Pack cuadernos + destacadores",
                    Desc = "5 cuadernos college + 6 destacadores.",
                    Price = 6000m,
                    Category = "Útiles",
                    Loc = "Coquimbo",
                    Contact = "j.vende@ucn.cl",
                },
            };

            int i = 0;
            foreach (var it in items)
            {
                var owner = sellers[i++ % sellers.Count];

                var bs = new BuySell
                {
                    UserId = owner.Id,
                    User = owner,
                    Title = it.Title,
                    Description = it.Desc,
                    PublicationDate = now.AddDays(-(i % 3)),
                    Type = Types.BuySell,
                    IsActive = true,
                    statusValidation = StatusValidation.Published,

                    Price = it.Price,
                    Category = it.Category,
                    Location = it.Loc,
                    ContactInfo = it.Contact,
                };

                context.BuySells.Add(bs);
            }

            // <<< INICIO: BUYSELL "INPROCESS" SOLICITADO >>>
            var firstSeller = sellers.First();
            var inProcessBuySell = new BuySell
            {
                UserId = firstSeller.Id,
                User = firstSeller,
                Title = "Venta de apuntes",
                Description =
                    "Vendo todos mis apuntes de primer año de ing. civil. Están en PDF. El admin debe revisar que no sea material con copyright.",
                PublicationDate = now.AddDays(-1),
                Type = Types.BuySell,
                IsActive = true,
                statusValidation = StatusValidation.InProcess, // <- Estado solicitado

                Price = 15000m,
                Category = "Útiles",
                Location = "Digital (PDF)",
                ContactInfo = "apuntes.pendientes@ucn.cl",
            };
            context.BuySells.Add(inProcessBuySell);

            await context.SaveChangesAsync();
            Log.Information("DataSeeder: BuySell de ejemplo cargados ({Count})", items.Length + 1); // +1 por el nuevo
        }

        private static async Task SeedJobApplications(AppDbContext context)
        {
            var studentUser = await context.Users.FirstOrDefaultAsync(u =>
                u.Email == "estudiante@alumnos.ucn.cl"
            );
            var offers = await context.Offers.ToListAsync();
            if (studentUser == null || offers.Count < 3)
                return;
            var studentId = studentUser.Id;
            var applications = new List<JobApplication>
            {
                new JobApplication
                {
                    StudentId = studentId,
                    Student = studentUser,
                    JobOfferId = offers[4].Id,
                    JobOffer = offers[4],
                    Status = "Pendiente",
                    ApplicationDate = DateTime.UtcNow.AddDays(-2),
                },

                new JobApplication
                {
                    StudentId = studentId,
                    Student = studentUser,
                    JobOfferId = offers[3].Id,
                    JobOffer = offers[3],
                    Status = "Pendiente",
                    ApplicationDate = DateTime.UtcNow.AddDays(-7),
                },

                new JobApplication
                {
                    StudentId = studentId,
                    Student = studentUser,
                    JobOfferId = offers[0].Id,
                    JobOffer = offers[0],
                    Status = "Pendiente",
                    ApplicationDate = DateTime.UtcNow.AddDays(-5),
                },
                new JobApplication
                {
                    StudentId = studentId,
                    Student = studentUser,
                    JobOfferId = offers[1].Id,
                    JobOffer = offers[1],
                    Status = "Aceptada",
                    ApplicationDate = DateTime.UtcNow.AddDays(-3),
                },
                new JobApplication
                {
                    StudentId = studentId,
                    Student = studentUser,
                    JobOfferId = offers[2].Id,
                    JobOffer = offers[2],
                    Status = "Rechazada",
                    ApplicationDate = DateTime.UtcNow.AddDays(-1),
                },
            };
            await context.JobApplications.AddRangeAsync(applications);
            await context.SaveChangesAsync();
            Log.Information(
                "DataSeeder: Postulaciones de prueba cargadas ({Count})",
                applications.Count
            );
        }

        #region Reviews

        private static async Task SeedReviews(AppDbContext context)
        {
            // Obtener estudiantes y oferentes (empresas e individuales)
            var students = await context.Users
                .Where(u => u.UserType == UserType.Estudiante)
                .ToListAsync();

            var offerents = await context.Users
                .Where(u => u.UserType == UserType.Empresa || u.UserType == UserType.Particular)
                .ToListAsync();

            var publications = await context.Offers.ToListAsync();

            if (students.Count == 0 || offerents.Count == 0 || publications.Count == 0)
            {
                Log.Warning("DataSeeder: No se pueden crear reviews - faltan usuarios o publicaciones");
                return;
            }

            var now = DateTime.UtcNow;
            var reviews = new List<Review>();
            var faker = new Faker("es");

            // Crear 12 reviews (6 completas + 6 incompletas)
            for (int i = 0; i < 12; i++)
            {
                var student = students[i % students.Count];
                var offerent = offerents[i % offerents.Count];
                var publication = publications[i % publications.Count];

                // Las primeras 6 reviews están completas (más de 14 días desde creación)
                bool isCompleted = i < 6;

                var review = new Review
                {
                    StudentId = student.Id,
                    Student = student,
                    OfferorId = offerent.Id,
                    Offeror = offerent,
                    PublicationId = publication.Id,
                    Publication = publication,

                    // Evaluación del oferente hacia el estudiante
                    RatingForStudent = faker.Random.Int(1, 6),
                    CommentForStudent = GenerateOfferentComment(faker, i),
                    AtTime = faker.Random.Bool(0.7f),
                    GoodPresentation = faker.Random.Bool(0.8f),
                    IsReviewForStudentCompleted = true,

                    // Evaluación del estudiante hacia el oferente (completa solo si isCompleted)
                    RatingForOfferor = isCompleted ? faker.Random.Int(1, 6) : (int?)null,
                    CommentForOfferor = isCompleted ? GenerateStudentComment(faker, i) : null,
                    IsReviewForOfferorCompleted = isCompleted,

                    IsCompleted = isCompleted,
                    ReviewWindowEndDate = isCompleted ? now.AddDays(-15) : now.AddDays(10),
                    HasReviewForStudentBeenDeleted = false,
                    HasReviewForOfferorBeenDeleted = false,
                };

                reviews.Add(review);
            }

            await context.Reviews.AddRangeAsync(reviews);
            await context.SaveChangesAsync();

            Log.Information("DataSeeder: {Count} reviews creadas exitosamente (6 completas, 6 incompletas)", reviews.Count);
            // Lo genere casi todo con IA. Esto es innecesario ya que en el Service cada vez que se completa una Review se
            // actualiza el rating del usuario correspondiente. Pero aca como se generan las Rewiews manual sin pasar por
            // el service, se actualiza manualmente con el codigo de abajo.
            // Actualizar ratings de todos los usuarios involucrados
            Log.Information("DataSeeder: Actualizando ratings de estudiantes y oferentes...");

            var allUserIds = new HashSet<int>();
            foreach (var review in reviews)
            {
                allUserIds.Add(review.StudentId);
                allUserIds.Add(review.OfferorId);
            }

            foreach (var userId in allUserIds)
            {
                var user = await context.Users.FindAsync(userId);
                if (user == null) continue;

                double? averageRating = null;

                if (user.UserType == UserType.Estudiante)
                {
                    // Calcular promedio de ratings para estudiante
                    var studentReviews = await context.Reviews
                        .Where(r => r.StudentId == userId && r.RatingForStudent.HasValue)
                        .ToListAsync();

                    if (studentReviews.Any())
                    {
                        averageRating = studentReviews.Average(r => r.RatingForStudent!.Value);
                    }
                }
                else if (user.UserType == UserType.Empresa || user.UserType == UserType.Particular)
                {
                    // Calcular promedio de ratings para oferente
                    var offerorReviews = await context.Reviews
                        .Where(r => r.OfferorId == userId && r.RatingForOfferor.HasValue)
                        .ToListAsync();

                    if (offerorReviews.Any())
                    {
                        averageRating = offerorReviews.Average(r => r.RatingForOfferor!.Value);
                    }
                }

                user.Rating = averageRating ?? 0.0;
                context.Users.Update(user);
            }

            await context.SaveChangesAsync();
            Log.Information("DataSeeder: Ratings actualizados exitosamente para {Count} usuarios", allUserIds.Count);
        }

        private static string GenerateOfferentComment(Faker faker, int index)
        {
            var comments = new[]
            {
                "Excelente estudiante, muy responsable y puntual. Cumplió con todas las tareas asignadas de manera profesional.",
                "Buen desempeño general, aunque tuvo algunos retrasos menores. Muestra potencial y ganas de aprender.",
                "Estudiante proactivo y con gran iniciativa. Se adaptó rápidamente al equipo de trabajo.",
                "Cumplió con las expectativas. Mostró buena disposición y capacidad de trabajo en equipo.",
                "Destacable compromiso y dedicación. Entregó trabajos de calidad consistentemente.",
                "Muy buen desempeño técnico. Resolvió problemas de forma eficiente y creativa.",
                "Estudiante responsable, aunque podría mejorar en comunicación con el equipo.",
                "Excelente actitud y profesionalismo. Superó las expectativas en varios aspectos.",
                "Buen trabajo en general. Cumplió con los plazos establecidos y mostró interés genuino.",
                "Demostró habilidades técnicas sólidas y capacidad de aprendizaje rápido.",
                "Estudiante confiable y organizado. Muy buena experiencia trabajando con él/ella.",
                "Buen nivel de compromiso. Aportó ideas valiosas al proyecto y trabajó de manera independiente."
            };
            return comments[index % comments.Length];
        }

        private static string GenerateStudentComment(Faker faker, int index)
        {
            var comments = new[]
            {
                "Muy buena experiencia laboral. El ambiente de trabajo fue excelente y aprendí mucho durante el proceso.",
                "Excelente oportunidad de aprendizaje. El equipo fue muy acogedor y el proyecto interesante.",
                "Buena experiencia en general. Me permitió aplicar conocimientos de la universidad en un contexto real.",
                "Ambiente laboral positivo y enriquecedor. Recomendaría esta oportunidad a otros estudiantes.",
                "Gran oportunidad para desarrollar habilidades profesionales. Supervisión y apoyo constante.",
                "Experiencia muy formativa. Aprendí nuevas tecnologías y metodologías de trabajo.",
                "Buen ambiente de trabajo, aunque a veces faltó claridad en las instrucciones.",
                "Excelente organización y claridad en las expectativas. Muy profesional en todo momento.",
                "Oportunidad valiosa que me permitió crecer profesionalmente. Muy agradecido/a por la experiencia.",
                "Buena experiencia laboral. El proyecto fue desafiante y motivador.",
                "Ambiente colaborativo y respetuoso. Aprendí mucho sobre trabajo en equipo.",
                "Experiencia positiva que superó mis expectativas. Excelente mentoría y guía durante el proceso."
            };
            return comments[index % comments.Length];
        }
    }
}
#endregion
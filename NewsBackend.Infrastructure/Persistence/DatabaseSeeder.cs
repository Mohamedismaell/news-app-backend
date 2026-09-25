using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewsBackend.Application.Interfaces;
using NewsBackend.Domain.Entities;
using NewsBackend.Domain.Enums;

namespace NewsBackend.Infrastructure.Persistence;

public class DatabaseSeeder : IDbSeeder
{
    private const string SeedPassword = "Password123";

    private readonly NewsDbContext _context;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(NewsDbContext context, ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (await _context.Users.AnyAsync(cancellationToken))
        {
            _logger.LogInformation("Database already contains users; skipping demo data seeding.");
            return;
        }

        _logger.LogInformation("Seeding demo data for an empty database.");

        var categories = SeedCategories();
        _context.Categories.AddRange(categories);
        await _context.SaveChangesAsync(cancellationToken);

        var bySlug = categories.ToDictionary(c => c.Slug);

        var admin = SeedUser(
            "admin@demo.news",
            "admin",
            "Site Admin",
            "Manages content and users across the platform.",
            UserRole.Admin);
        var editor = SeedUser(
            "editor@demo.news",
            "editor",
            "Demo Editor",
            "Writes and curates the latest stories.",
            UserRole.Editor);
        var reader = SeedUser(
            "user@demo.news",
            "reader",
            "Demo Reader",
            "Just here for the headlines.",
            UserRole.User);

        _context.Users.AddRange(admin, editor, reader);
        await _context.SaveChangesAsync(cancellationToken);

        var authors = new Dictionary<string, User>
        {
            ["admin"] = admin,
            ["editor"] = editor,
            ["reader"] = reader
        };

        var today = DateTime.UtcNow;
        var articles = new List<Article>();
        foreach (var (categorySlug, status, title, summary, content, authorUsername, imageSeed, ageDays) in SeedArticles())
        {
            var author = authors[authorUsername];
            articles.Add(new Article
            {
                Title = title,
                Summary = summary,
                Content = content,
                ImageUrl = imageSeed is null ? null : $"https://picsum.photos/seed/{imageSeed}/800/450",
                Status = status,
                PublishedAt = status == ArticleStatus.Published ? today.AddDays(-ageDays) : null,
                CreatedAt = today.AddDays(-ageDays - 30),
                UpdatedAt = today.AddDays(-ageDays),
                Category = bySlug[categorySlug],
                Author = author
            });
        }

        _context.Articles.AddRange(articles);
        await _context.SaveChangesAsync(cancellationToken);

        var published = articles.Where(a => a.Status == ArticleStatus.Published).ToList();

        var comments = new List<Comment>
        {
            new()
            {
                Content = "Great overview — I had no idea the ecosystem had moved this far.",
                Article = published[0],
                User = reader
            },
            new()
            {
                Content = "Would love to see a follow-up with real-world benchmarks.",
                Article = published[0],
                User = reader
            },
            new()
            {
                Content = "Solid points, though I would argue timing matters just as much as the stack.",
                Article = published[1],
                User = reader
            },
            new()
            {
                Content = "Bookmarking this one for the team email this week.",
                Article = published[2],
                User = reader
            }
        };

        _context.Comments.AddRange(comments);
        await _context.SaveChangesAsync(cancellationToken);

        var bookmarks = new List<Bookmark>
        {
            new() { Article = published[0], User = reader },
            new() { Article = published[1], User = reader },
            new() { Article = published[3], User = reader }
        };

        _context.Bookmarks.AddRange(bookmarks);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Demo data seeded: 3 users, 5 categories, {ArticleCount} articles, {CommentCount} comments, {BookmarkCount} bookmarks. Demo login: admin@demo.news / Password123.",
            articles.Count,
            comments.Count,
            bookmarks.Count);
    }

    private static List<Category> SeedCategories()
    {
        return
        [
            new Category
            {
                Name = "Technology",
                Slug = "technology",
                Description = "Software, hardware, and the people building the future."
            },
            new Category
            {
                Name = "Sports",
                Slug = "sports",
                Description = "Live results, analysis, and stories from the world of sport."
            },
            new Category
            {
                Name = "Business",
                Slug = "business",
                Description = "Markets, companies, and the forces shaping the economy."
            },
            new Category
            {
                Name = "Science",
                Slug = "science",
                Description = "Discovery, research, and what the data really says."
            },
            new Category
            {
                Name = "Entertainment",
                Slug = "entertainment",
                Description = "Film, music, television, and everything in between."
            }
        ];
    }

    private static User SeedUser(string email, string username, string displayName, string bio, UserRole role)
    {
        return new User
        {
            Email = email,
            Username = username,
            DisplayName = displayName,
            Bio = bio,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(SeedPassword),
            Role = role
        };
    }

    private static IEnumerable<(string CategorySlug, ArticleStatus Status, string Title, string Summary, string Content, string AuthorUsername, string? ImageSeed, int AgeDays)> SeedArticles()
    {
        return
        [
            (
                "technology",
                ArticleStatus.Published,
                "Redis Finds Its Way Back into Every Architecture Discussion",
                "Why the in-memory data store still powers caching, queues, and sessions at internet scale.",
                "It is easy to dismiss Redis as just a cache, yet it sits behind almost every large web application you use. Beyond simple key-value lookups, it powers rate limiting, distributed locks, pub/sub messaging, and ephemeral queues that hold up well under load.\n\nWhat makes Redis attractive today is its simplicity. A small, predictable tool with a tiny learning curve solves real production problems without pulling in a heavier platform. Teams keep rediscovering it the moment their writes slow down or their sessions need to scale horizontally.\n\nThe ecosystem has matured as well: cluster mode, ACLs, and safer persistence options mean it can move beyond the toy-project stage. If your API is already serving thousands of requests per minute, Redis is probably the most pragmatic next step you can take.",
                "editor",
                "redis",
                0
            ),
            (
                "technology",
                ArticleStatus.Published,
                "Open Source Licenses Explained Without the Lawyers",
                "MIT, Apache 2.0, GPL, and AGPL — what each one actually allows you to do.",
                "Every library you install carries a license, and most developers never read it. The difference between permissive and copyleft licenses matters far more than most realize, especially once a product ships.\n\nThe MIT license is the most permissive of the common options: use it, modify it, sell it, as long as you keep the attribution notice. Apache 2.0 adds an explicit patent grant, which is why companies tend to prefer it for infrastructure code.\n\nThe GPL family is where things get serious. If you distribute a modified GPL project, your changes must remain open under the same license. AGPL extends this obligation to software you only serve over a network. For an API-company, that distinction is the one worth memorizing first.",
                "editor",
                "opensource",
                1
            ),
            (
                "business",
                ArticleStatus.Published,
                "The Quiet Shift Toward Carbon-Neutral Supply Chains",
                "Corporations are quietly rewriting their logistics around emissions data, not just cost.",
                "For years, supply chains were optimised around a single variable: cost per unit. Today the second variable is carbon. Buyers increasingly demand emissions data per shipment, and logistics providers that cannot produce it are losing contracts before the negotiation starts.\n\nSoftware is doing the heavy lifting. Telemetry from trucks, ships, and warehouses is fed into models that estimate real emissions per kilometre. The numbers are imperfect, but they are good enough to change behaviour — carriers reroute, consolidate, and slow-steam because the data says so.\n\nThe shift is not driven by regulation so much as by procurement teams adding a line to their scoring matrix. That is quieter than any law, and harder to fight.",
                "editor",
                "supplychain",
                2
            ),
            (
                "business",
                ArticleStatus.Published,
                "Five Practical Ways to Cut Energy Costs in the Office",
                "Small operational tweaks that add up to a surprisingly large line on the budget.",
                "Energy is rarely a headline cost for an office, which is exactly why it leaks money. Lighting, HVAC, and idle equipment quietly consume the majority of a typical building's bill.\n\nThe cheapest wins are behavioural: switch meeting rooms to presence sensors, enable automatic power-down on desktops, and schedule the HVAC around actual occupancy rather than fixed hours. None of these require capital investment and all of them show up on the next bill.\n\nAfter that, submetering tells you where to spend next. When you can see that the server room uses forty percent of the building's power, the business case for consolidation writes itself. The most expensive energy is always the energy nobody measured.",
                "editor",
                "energy",
                3
            ),
            (
                "science",
                ArticleStatus.Published,
                "Why Deep Learning Wants More Data, Not Just Better Models",
                "Architecture improvements matter, but measured data quality moves the needle faster.",
                "Every new model release is a story about the network, yet the real story is the dataset. Time after time, adding high-quality labeled examples beats a cleverer architecture with the same training data.\n\nThis is not glamorous work. It means cleaning outliers, relabeling ambiguous cases, and documenting how each example was produced. But the discipline pays off: a well-curated ten thousand examples can outperform a sloppy million.\n\nThe pattern holds across domains, from computer vision to language. Researchers increasingly share not just weights but their data pipelines. As the field matures, data engineering is quietly becoming the highest-leverage skill in applied machine learning.",
                "editor",
                "datalabour",
                4
            ),
            (
                "science",
                ArticleStatus.Published,
                "Long-Duration Energy Storage Gets Its First Serious Moment",
                "Grids are beginning to buy storage measured in days, not hours.",
                "Wind and solar are cheap, but they are intermittent. Lithium-ion batteries cover hours, not seasons. Long-duration storage — flow batteries, compressed air, thermal — is the piece that could finally let a grid run mostly on renewables.\n\nPilots have been running for years, but something changed recently: orders. Utilities are signing contracts measured in gigawatt-hours, and manufacturers are building factories around those commitments rather than around research grants.\n\nThe economics are simple enough. If storage can shift energy across days at a per-kilowatt-hour cost below today's peaking plants, it gets bought. The race now is manufacturing cost, not chemistry novelty.",
                "editor",
                "storage",
                5
            ),
            (
                "sports",
                ArticleStatus.Published,
                "How Streaming Changed the Way We Watch Sports",
                "Fans no longer follow a team; they follow a league inside an app.",
                "The stadium is still the cathedral, but the habit moved to a streaming tab. Broadcasters and leagues have rebuilt their products around subscriptions, clips, and second screens, and the numbers say the model works.\n\nFor fans the change is about control: watch at your own pace, choose your own camera angle, and get highlights in the same feed as your chat. For leagues it is about data — every stream tells them who watches, when they drop off, and which moments they replay.\n\nThe trade-off is fragmentation. Follow two sports and you may need three subscriptions. The next wave of bundling is already starting to undo that, in much the same way cable once did.",
                "editor",
                "streaming",
                6
            ),
            (
                "entertainment",
                ArticleStatus.Published,
                "Indie Films Are Winning Audiences Back, One Small Release at a Time",
                "Niche stories are carving out real revenue away from the blockbuster pipeline.",
                "The box office conversation is dominated by franchises, yet the healthiest part of the industry are the small films nobody opened with a billion-dollar weekend expectation.\n\nIndie distribution now runs through the streaming catalogues that need depth as much as they need premieres. A modest thriller with a strong hook can find its audience through recommendations alone, and its economics stay manageable because its costs never explode.\n\nThe audience is the surprise. Year after year, the films with no marquee star but a sharp idea keep overperforming on rent-per-view. The lesson studios keep relearning is that people pay for a good story, not just a big one.",
                "editor",
                "indiefilm",
                7
            ),
            (
                "technology",
                ArticleStatus.Draft,
                "Inside the Race to Build a Useful Quantum Computer",
                "Notes from the lab: what 'useful' means is changing faster than the hardware.",
                "Quantum computing's timeline has been 'ten years away' for thirty years, which is exactly the kind of joke the field tells on itself. What has changed is that companies now benchmark against real business problems instead of physics milestones.\n\nThe current generation is noisy, but error mitigation is improving quickly. Researchers have begun treating these machines as accelerators for specific optimisation problems rather than as general-purpose supercomputers.\n\nThe draft paints a picture of cautious optimism. The stack — hardware, control electronics, error correction, algorithms — is moving together for the first time, and just one milestone could shift the entire conversation.",
                "editor",
                "quantum",
                2
            ),
            (
                "business",
                ArticleStatus.Draft,
                "Understanding Inflation When Interest Rates Stop Moving",
                "A plain-language look at what happens after the policy cycle settles.",
                "The interesting phase of the inflation story may be the one nobody is writing about: what happens when rates stop rising. Consumers have started to bargain-hunt again, and businesses are learning to absorb costs instead of passing them on.\n\nWages, housing, and energy have decoupled in ways that make single-number headlines misleading. Core inflation is no longer one conversation.\n\nA good article here is less about prediction and more about the frameworks readers should use to interpret the claims they see. Expect the draft to land on cautious analysis rather than certainty.",
                "editor",
                "inflation",
                1
            ),
            (
                "science",
                ArticleStatus.Draft,
                "The Science of Sleeping Better, Backed by Data",
                "What actually improves sleep for most people, in order of measured impact.",
                "Sleep research is full of gadgets and few controlled comparisons. The surprising part is that the highest-impact interventions are almost embarrassingly simple.\n\nConsistent wake times outperform fancy trackers. Bright light in the morning moves the circadian clock faster than any supplement. And the room temperature twilight zone — 17 to 19 degrees Celsius — is where most people fall asleep fastest.\n\nThe draft organizes these findings by effect size rather than fashion, which is what makes it worth publishing rather than trending.",
                "editor",
                "sleep",
                0
            )
        ];
    }
}
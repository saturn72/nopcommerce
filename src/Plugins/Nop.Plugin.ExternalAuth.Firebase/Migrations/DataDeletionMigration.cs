using FluentMigrator;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Services.Localization;
using Nop.Web.Framework.Extensions;

namespace Nop.Plugin.ExternalAuth.Firebase.Migrations
{
    [NopMigration("2022-06-23 00:00:00", "ExternalAuth.Firebase 1.77. Data deletion feature", MigrationProcessType.Update)]
    public class DataDeletionMigration : MigrationBase
    {
        #region Fields

        protected readonly ILanguageService _languageService;
        protected readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor

        public DataDeletionMigration(ILanguageService languageService,
            ILocalizationService localizationService)
        {
            _languageService = languageService;
            _localizationService = localizationService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
        }

        /// <summary>
        /// Collects the DOWN migration expressions
        /// </summary>
        public override void Down()
        {
            //nothing
        }

        #endregion
    }
}
import { Helmet } from 'react-helmet';
export const baseURL = 'https://kozijn-magazijn-website.com';

const MetaTags = ({ children, OG_TITLE, OG_DESCRIPTION, OG_URL, CANONICAL, TWITTER_TITLE, TWITTER_DESCRIPTION }) => {
  return (
    <Helmet>
      <link rel='canonical' href={`${baseURL}${CANONICAL}`} />
      <meta name='description' content={OG_DESCRIPTION} />
      <meta property='og:title' content={OG_TITLE} key='ogtitle' />
      <meta property='og:description' content={OG_DESCRIPTION} key='ogdesc' />
      <meta property='og:image' content={`/assets/img/logo.png`} />
      <meta property='og:type' content='website' />
      <meta property='og:url' content={`${baseURL}${OG_URL}`} />
      <meta name='twitter:card' content='summary' />
      <meta name='twitter:site' content={baseURL} />
      <meta name='twitter:title' content={TWITTER_TITLE} />
      <meta name='twitter:description' content={TWITTER_DESCRIPTION} />
      <meta name='twitter:image' content={`/assets/img/logo.png`} />
      <title>{OG_TITLE}</title>
      {children}
    </Helmet>
  );
};

export default MetaTags;

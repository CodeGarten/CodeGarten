package CodeGarten::AuthzHandler;
  
  use strict;
  use warnings;
  
  use Apache2::Access ();
  use Apache2::RequestUtil ();
  use Apache2::RequestRec ();
  use Apache2::RequestIO ();
  
  use XML::XPath;
  use XML::XPath::XMLParser;
  
  use Switch;
  
  use Apache2::Const -compile => qw(OK HTTP_UNAUTHORIZED);
  
  sub handler {
      my $r = shift;
	  
	  return user_is_authorized($r->user, $r->unparsed_uri, $r->dir_config('GitPermissions'))?
	  Apache2::Const::OK : Apache2::Const::HTTP_UNAUTHORIZED;
  }
  
  sub user_is_authorized
	{
		my ($user, $uri, $git_permissions) = @_;
		
		my $auth_xmlFile = XML::XPath->new(filename => $git_permissions); 
		my ($repo, $perm) = repo_perm_type ($uri);
		return is_user_autho ($auth_xmlFile, $user, $perm, $repo );
	}
   
	sub repo_perm_type
	{
		my $url = shift;
		my $regex = '.*/git/(.*).git/info/refs\?service=(.*)';
		my ($repo, $action) = $url =~ $regex;
		
		switch($action) {
          case (/git-upload-pack/) {return ($repo,'r');}
          case (/git-receive-pack/) {return ($repo,'rw');}
		}
	}
	
	sub is_user_autho
	{
		my ($xml, $user, $perm, $locationRepo) = @_;
		
		$perm= $perm.'\' or @perm=\'rw' if (length($perm)==1);
			
		my $xpath_group = 'autho_file/group[@ID=//repo[@location=\''.$locationRepo.'\']/group[@perm=\''.$perm.'\']/text()]/user[.=\''.$user.'\' or .=\'*\']';
		my $xpath_user = 'autho_file/repo[@location=\''.$locationRepo.'\']/user[@perm=\''.$perm.'\' and (.=\'*\' or .=\''.$user.'\')]';
		
		return $xml->exists($xpath_user) || $xml->exists($xpath_group);
	}
  1;
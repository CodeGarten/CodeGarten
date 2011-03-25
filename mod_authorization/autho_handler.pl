#!C:\Perl\bin\perl

use warnings;
use strict;

use XML::XPath;
use XML::XPath::XMLParser;
use XML::Simple;
    #########################################################
	###		Testes                                        ###
	#########################################################
	
	my $user = "CodeGarten";
	my $url = "ola/git/cena2/git-upload-pack";

	print "Teste- ".user_is_authorized($user, $url);
	
	
	#########################################################
	###			                                          ###
	#########################################################
	
   
	# função que veririfa se o utilizador está autentorizado
	# args : userName , uri
	sub user_is_authorized
	{
		my $user = $_[0];
		my $uri = $_[1];
		my $auth_xmlFile = XML::XPath->new(filename => 'autho_file.xml');
		my $conv_xmlFile = XMLin( 'converter_file.xml' ); 
		my ($repo, $perm) = get_perm_and_repo ($conv_xmlFile, $uri);
		return user_is_autho ($auth_xmlFile, $user, $perm, $repo );
	}
   
	sub get_perm_and_repo
	{
		my $xml = $_[0];
		my $url = $_[1];
		
		my $locations = $xml->{LocationMatch};
		my $lengthLocations =  @$locations;
		
		for( my $i = 0; $i < $lengthLocations; $i++ ) 
		{
			my $regex =  $xml->{LocationMatch}[$i]->{regex};
			
			if ($url =~ $regex){
				my ($repo) = ($url =~ $regex);
				my $perm = $xml->{LocationMatch}[$i]->{perm};
				return ($repo, $perm);
			}
			
		}
				
	}
	
	sub user_is_autho
	{
		my $xml = $_[0];
		my $user = $_[1];
		my $perm = $_[2];
		
		my $locationRepo = $_[3];
		
		return 1 if ($perm eq '*');
		
		$perm= $perm.'\' or @perm=\'rw' if (length($perm)==1);
			
		my $xpath_group = 'autho_file/group[@ID=//repo[@location=\''.$locationRepo.'\']/group[@perm=\''.$perm.'\']/text()]/user[.=\''.$user.'\' or .=\'*\']';
		my $xpath_user = 'autho_file/repo[@location=\''.$locationRepo.'\']/user[@perm=\''.$perm.'\' and (.=\'*\' or .=\''.$user.'\')]';
		
		return 1 if ($xml->exists($xpath_user));
			
		return $xml->exists($xpath_group);
	}
	